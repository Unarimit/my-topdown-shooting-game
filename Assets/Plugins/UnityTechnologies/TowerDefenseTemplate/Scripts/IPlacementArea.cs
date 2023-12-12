using UnityEngine;

namespace TowerDefense.Towers.Placement
{
    public enum TowerFitStatus
    {
        /// <summary>
        /// Tower fits in this location
        /// </summary>
        Fits,

        /// <summary>
        /// Tower overlaps another tower in the placement area
        /// </summary>
        Overlaps,

        /// <summary>
        /// Tower exceeds bounds of the placement area
        /// </summary>
        OutOfBounds
    }
    /// <summary>
    /// An interface for a placement area that can contain a tower
    /// </summary>
    public interface IPlacementArea
	{
		/// <summary>
		/// Gets this object's transform
		/// </summary>
		Transform transform { get; }

		/// <summary>
		/// Calculates the grid position from a given world position, offset to center for a specific size object
		/// </summary>
		Vector2Int WorldToGrid(Vector3 worldPosition, Vector2Int sizeOffset);

		/// <summary>
		/// Calculates the snapped world position from a given grid position
		/// </summary>
		Vector3 GridToWorld(Vector2Int gridPosition, Vector2Int sizeOffset);

		/// <summary>
		/// Gets whether an object of a given size would fit on this grid at the given location
		/// </summary>
		/// <param name="gridPos">The grid location</param>
		/// <param name="size">The size of the item</param>
		/// <returns>True if the item would fit at <paramref name="gridPos"/></returns>
		TowerFitStatus Fits(Vector2Int gridPos, Vector2Int size);

		/// <summary>
		/// Occupy the given space on this placement area
		/// </summary>
		/// <param name="gridPos">The grid location</param>
		/// <param name="size">The size of the item</param>
		void Occupy(Vector2Int gridPos, Vector2Int size);

		/// <summary>
		/// Clear the given space on this placement area
		/// </summary>
		/// <param name="gridPos">The grid location</param>
		/// <param name="size">The size of the item</param>
		void Clear(Vector2Int gridPos, Vector2Int size);
	}

	public static class PlacementAreaExtensions
	{
		/// <summary>
		/// Snaps a given world positionn to this grid
		/// </summary>
		public static Vector3 Snap(this IPlacementArea placementArea, Vector3 worldPosition, Vector2Int sizeOffset)
		{
			// Calculate the nearest grid location and then change that back to world space
			return placementArea.GridToWorld(placementArea.WorldToGrid(worldPosition, sizeOffset), sizeOffset);
		}
	}
}