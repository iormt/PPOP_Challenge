using System.Collections.Generic;

namespace PathFinding
{
	/// <summary>
	/// Interface for classes that can be used with the A* algorithm defined in AStar script.
	/// The navigation targets in the world have to inherit from this interface for the algorithm to work.
	/// </summary>
	public interface IAStarNode
	{
		/// <summary>
		/// The neighbours property returns an enumeration of all the nodes adjacent to this node.
		/// </summary>
		IEnumerable<IAStarNode>	Neighbours
		{
			get;
		}

		/// <summary>
		/// This method should calculate the exact cost of travelling from this node to neighbour node.
		/// When the A* algorithm calls this method, the neighbour parameter is guaranteed 
		/// to be one of the nodes in the Neighbours property.
		/// </summary>
		/// <returns>
		/// The returned 'cost' could be a distance, time or any other countable value, 
		/// where smaller is considered better by the algorithm.
		/// </returns>
		float CostTo(IAStarNode neighbour);

		/// <summary>
		/// This method should estimate the distance to travel from this node to target. Target may be
		/// any node in the graph, so there is no guarantee it is a direct neighbour. The better the estimation
		/// the faster the AStar algorithm will find the optimal route. Be careful however, the cost of calculating
		/// this estimate does not outweigh any benefits for the AStar search.
		/// </summary>
		/// <returns>
		/// Cost to the target node. This cost could be distance, time, 
		/// or any other countable value, where smaller is considered better by the algorithm.
		/// The estimate needs to consistent. (Also know as monotone)
		/// </returns>
		float EstimatedCostTo(IAStarNode target);
	}
}