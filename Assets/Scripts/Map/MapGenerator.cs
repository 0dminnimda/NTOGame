using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static Map.Direction;
using static Map.DirectionExt;
using Random = UnityEngine.Random;

namespace Map
{
	public class MapGenerator : MonoBehaviour
	{
		/// <summary>
		/// The number of iterations the generator should go through.
		/// </summary>
		public int iterations = 10;

		/// <summary>
		/// the AreaData templates that will be used in generating the scene.
		/// </summary>
		AreaData[] areaDatas =
		{
			// full fours
			new AreaData("N E S W", new[] {N, E, S, W}),
			new AreaData("N E S W", new[] {N, E, S, W}),

			// triplets
			new AreaData("N E S", new[] {N, E, S}),
			new AreaData("E S W", new[] {E, S, W}),
			new AreaData("S W N", new[] {S, W, N}),
			new AreaData("W N E", new[] {W, N, E}),

			// turns
			new AreaData("W N", new[] {W, N}),
			new AreaData("N E", new[] {N, E}),
			new AreaData("E S", new[] {E, S}),
			new AreaData("S W", new[] {S, W}),

			// straight paths
			new AreaData("E W", new[] {E, W}),
			new AreaData("N S", new[] {N, S}),
		};

		private Dictionary<int, int> angle2turn3 = new Dictionary<int, int>
		{
			{DirectionArrayHash(new [] {W, N}), -90},
			{DirectionArrayHash(new [] {N, E}), 0},
			{DirectionArrayHash(new [] {E, S}), 90},
			{DirectionArrayHash(new [] {S, W}), 180},
			{DirectionArrayHash(new [] {N, E, S}), 180},
			{DirectionArrayHash(new [] {E, S, W}), -90},
			{DirectionArrayHash(new [] {S, W, N}), 0},
			{DirectionArrayHash(new [] {W, N, E}), 90},
		};

		/// <summary>
		/// The areas generated by Generate(). 
		/// </summary>
		public List<AreaData> generatedAreas { get; set; }

		public GameObject[] areaOne; 
		public GameObject[] areaTwoStraight;
		public GameObject[] areaTwoTurn;
		public GameObject[] areaThree;
		public GameObject[] areaFour;
		
		private List<GameObject> generatedObjects;

		/// <summary>
		/// Is called when this component is added to a GameObject. 
		/// Sets up some default areas.
		/// </summary>
		void Reset()
		{
			// Set up some default areas
			areaDatas = new AreaData[4];
			int transitionCount = System.Enum.GetNames(typeof(Direction)).Length;
			for (int i = 0; i < areaDatas.Length; i++)
			{
				int minTransition = Random.Range(1, transitionCount);
				int transitionStart = Random.Range(minTransition, transitionCount);
				Direction[] transitions = new Direction[transitionCount - transitionStart];

				int counter = 0;
				for (int j = transitionStart; j < transitionCount; j++)
				{
					transitions[counter] = (Direction) j;
					counter++;
				}

				areaDatas[i] = new AreaData(AreaData.GetTransitionList(transitions), transitions);
			}
		}

		AreaData InitialAreas()
		{
			generatedAreas = new List<AreaData>();

			(String, Direction[], Coordinates)[] data =
#if false
		{
			("Reactor",    new [] {N, E, S, W}, new Coordinates( 0,  0)),
			("Reactor-l",  new [] {N, E, S, W}, new Coordinates(-1,  0)),
			("Reactor-r",  new [] {N, E, S, W}, new Coordinates( 1,  0)),
			("Reactor-u",  new [] {N, E, S, W}, new Coordinates( 0,  1)),
			("Reactor-d",  new [] {N, E, S, W}, new Coordinates( 0, -1)),
			("Reactor-ul", new [] {N, E, S, W}, new Coordinates(-1,  1)),
			("Reactor-ur", new [] {N, E, S, W}, new Coordinates( 1,  1)),
			("Reactor-dl", new [] {N, E, S, W}, new Coordinates(-1, -1)),
			("Reactor-dr", new [] {N, E, S, W}, new Coordinates( 1, -1)),
		};
#else
			{
				("Reactor-ul", new[] {N, E, S, W}, new Coordinates(0, 1)),
				("Reactor-ur", new[] {N, E, S, W}, new Coordinates(1, 1)),
				("Reactor-dl", new[] {N, E, S, W}, new Coordinates(0, 0)),
				("Reactor-dr", new[] {N, E, S, W}, new Coordinates(1, 0)),
			};
#endif

			AreaData area = null;
			foreach ((string name, Direction[] dir, Coordinates coord) in data)
			{
				area = new AreaData(name, dir);
				area.coordinates = coord;
				generatedAreas.Add(area);
			}

			if (area != null)
				return area;
			else
			{
				// Select a random area to start with.
				int randomAreaIndex = Random.Range(0, areaDatas.Length);
				AreaData randomArea = areaDatas[randomAreaIndex];

				// Create a copy of the AreaData.
				return new AreaData(randomArea.name, randomArea.availableTransitions);
			}
		}

		void Generate()
		{
			AreaData newArea = InitialAreas();

			for (int i = 0; i < iterations; i++)
			{
				// Iterate through this area's transitions and add connections
				// however, we'll also need to check for existing areas blocking the way or that should be connected
				for (int j = 0; j < newArea.availableTransitions.Length; j++)
				{
					Direction transition = newArea.availableTransitions[j];

					if (transition == Direction.None)
						continue;

					Direction opposite = transition.GetOpposite();

					Coordinates adjacentAreaCoordinate = newArea.coordinates.GetAdjacentCoordinate(transition);
					AreaData adjacentArea = GetGeneratedAreaByCoordinate(adjacentAreaCoordinate);

					// if there's an area in the way check if it has an available transition opposite of this transition.
					if (adjacentArea != null)
					{
						if (!adjacentArea.GetIsTransitionAvailable(opposite))
						{
							// The adjecent area cannot be transitioned to from this area.
							adjacentArea = null;

							// We should actually now flag this direction as no longer viable.
							newArea.availableTransitions[j] = Direction.None;
						}
					}
					// otherwise create a new area
					else
					{
						adjacentArea = CreateRandomAreaWithTransition(opposite);

						if (adjacentArea == null)
						{
							Debug.LogErrorFormat(
								"Could not GetRandomAreaWithTransition({0}). " +
								"Please ensure areaDatas has available transitions on all sides",
								opposite);
						}
						else
						{
							adjacentArea.coordinates = adjacentAreaCoordinate;
							generatedAreas.Add(adjacentArea);
						}
					}

					if (adjacentArea != null)
					{
						// assign the connection between the two areas.
						newArea.SetTransitionUsed(transition, adjacentArea, opposite);
						adjacentArea.SetTransitionUsed(opposite, newArea, transition);
					}
				}

				// check to see if we assigned any transitions to this new area, if so add it to the generatedAreas list.
				if (newArea.GetTransitionCount() > 0)
				{
					if (!generatedAreas.Contains(newArea))
						generatedAreas.Add(newArea);
				}
				// otherwise did something go wrong?
				else
				{
					Debug.LogWarning("No transitions assigned to area: " + newArea.ToString());
				}

				// Now we need to get the next area to work on.
				newArea = null;
				foreach (var item in generatedAreas)
				{
					if (item.HasAnyAvailableTransition())
					{
						newArea = item;
						break;
					}
				}

				if (newArea == null)
				{
					Debug.Log("Can't find any generated areas with avilable transitions. Quitting.");
					break;
				}
			}

			CreateMeshes();
		}


		/// <summary>
		/// Checks the list of generated areas to see if one exists at the supplied coordinates.
		/// </summary>
		/// <param name="coordinates">Coordinates to check if an area exists at.</param>
		/// <returns>An area from the generated areas list matching the supplied coordinates. 
		/// If none is found, then null is returned.</returns>
		private AreaData GetGeneratedAreaByCoordinate(Coordinates coordinates)
		{
			foreach (var item in generatedAreas)
			{
				if (item.coordinates.x == coordinates.x && item.coordinates.y == coordinates.y)
					return item;
			}

			return null;
		}


		/// <summary>
		/// Creates a new random area with the indicated position available.
		/// </summary>
		/// <param name="transition">The transition that needs to be available on the area.</param>
		/// <returns>A new AreaData with matching transition. If none can be found then null is returned.</returns>
		private AreaData CreateRandomAreaWithTransition(Direction transition)
		{
			int areaDatasIndex = Random.Range(0, areaDatas.Length);

			//Debug.Log("transition to look for: " + transition);

			for (int i = 0; i < areaDatas.Length; i++)
			{
				bool isTransitionAvailable = areaDatas[areaDatasIndex].GetIsTransitionAvailable(transition);
				//Debug.LogFormat("areaDatasIndex: {0}  areaData {1}  available: {2}", areaDatasIndex, areaDatas[areaDatasIndex], isTransitionAvailable);

				if (isTransitionAvailable)
					return new AreaData(
						areaDatas[areaDatasIndex].name,
						areaDatas[areaDatasIndex].availableTransitions);

				areaDatasIndex++;
				if (areaDatasIndex == areaDatas.Length)
					areaDatasIndex = 0;
			}

			return null;
		}


		void Start()
		{
			Generate();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				foreach (GameObject o in generatedObjects)
				{
					Destroy(o);
				}

				generatedObjects.Clear();
				Generate();
			}
		}

		/// <summary>
		/// Creates cubes for each generated area and cubes to show the transitions between each.
		/// Transitions are offset so that we can see 1 exists in each direction (to/from).
		/// </summary>
		void CreateMeshes()
		{
			generatedObjects = new List<GameObject>();

			foreach (var area in generatedAreas)
			{
				int angle;
				GameObject d;
				var key = area.transitions.First().Key;
				//area.transitions = area.transitions.OrderBy(x => x.Key).ToDictionary(x => x);

				var t = area.Type();
				switch (t)
				{
					case AreaType.One:
						d = areaOne[0];         angle = area.transitions.First().Key.GetAngle() + 180;
						break;
					case AreaType.TwoStraight:
						d = areaTwoStraight[0]; angle = area.transitions.First().Key.GetAngle();
						break;
					case AreaType.TwoTurn:
						d = areaTwoTurn[0];     angle = angle2turn3[DirectionArrayHash(area.transitions.Keys)];
						break;
					case AreaType.Three:
						d = areaThree[0];       angle = angle2turn3[DirectionArrayHash(area.transitions.Keys)];
						break;
					case AreaType.Four:
						d = areaFour[0];        angle = 0;
						break;
					default:
						throw new System.Exception(
							"Invalid area type: " +
							area.Type().ToString());
				}

				CreateArea(d, area, angle);
			}
		}

		private void CreateArea(GameObject d, AreaData area, int angle)
		{
			//angle = area.availableTransitions[0].GetAngle();
			GameObject o = Instantiate(d, area.coordinates.ToVector3() + Vector3.down / 2,
				Quaternion.Euler(0, angle, 0));

			generatedObjects.Add(o);
			// Attach an Area component so we can easily inspect the AreaData in the editor.
			DebugArea dbArea = o.AddComponent<DebugArea>();
			dbArea.areaData = area;
			// cube.transform.localScale = 0.75f * Vector3.one;
			o.name = area.name;

			// foreach (var item in area.transitions)
			// {
			// 	Vector3 transitionPostion =
			// 		0.5f * (area.coordinates.ToVector3() + item.Value.Key.coordinates.ToVector3());
			//
			// 	GameObject transition = GameObject.CreatePrimitive(PrimitiveType.Cube);
			// 	Vector3 scale = 0.125f * Vector3.one;
			// 	transition.name = item.Key.ToString() + " to " + item.Value.Value.ToString();
			//
			// 	switch (item.Key)
			// 	{
			// 		case Direction.N:
			// 			transitionPostion.x += 0.125f;
			// 			scale.z = 0.5f;
			// 			break;
			// 		case Direction.E:
			// 			transitionPostion.z += 0.125f;
			// 			scale.x = 0.5f;
			// 			break;
			// 		case Direction.S:
			// 			scale.z = 0.5f;
			// 			transitionPostion.x -= 0.125f;
			// 			break;
			// 		case Direction.W:
			// 			transitionPostion.z -= 0.125f;
			// 			scale.x = 0.5f;
			// 			break;
			// 		default:
			// 			break;
			// 	}
			//
			// 	transition.GetComponent<MeshRenderer>().material.color = Color.green;
			// 	transition.transform.position = transitionPostion;
			// 	transition.transform.localScale = scale;
			// 	transition.transform.SetParent(o.transform, true);
			// }
		}
	}
}
