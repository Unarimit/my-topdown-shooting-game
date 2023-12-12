using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Towers.Placement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.HomeLogic.Placement
{
    internal class PlacementController : MonoBehaviour
    {   
        /// <summary>
        /// Placement area ghost tower is currently on
        /// </summary>
        IPlacementArea m_CurrentArea;
        /// <summary>
		/// Grid position ghost tower in on
		/// </summary>
		Vector2Int m_GridPosition;
        /// <summary>
		/// Current tower placeholder. Will be null if not in the <see cref="State.Building" /> state.
		/// </summary>
		public BuildingPlaceController m_CurrentBuilding;
        /// <summary>
		/// Tracks if the ghost is in a valid location and the player can afford it
		/// </summary>
		bool m_GhostPlacementPossible;

        private void Update()
        {
            MoveGhost(false);
        }

        /// <summary>
		/// Move the ghost to the pointer's position
		/// </summary>
		/// <param name="pointer">The pointer to place the ghost at</param>
		/// <param name="hideWhenInvalid">Optional parameter for whether the ghost should be hidden or not</param>
		/// <exception cref="InvalidOperationException">If we're not in the correct state</exception>
		protected void MoveGhost(bool hideWhenInvalid = true)
        {
            if (m_CurrentBuilding == null) //  || !isBuilding
            {
                throw new InvalidOperationException(
                    "Trying to position a tower ghost while the UI is not currently in the building state.");
            }

            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            var hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask(new string[] { "BuildingGrid" }));

            if (hits.Length == 1)
            {
                MoveGhostWithRaycastHit(hits[0]);
            }
            else
            {
                MoveGhostOntoWorld(ray, hideWhenInvalid);
            }
        }


        private void MoveGhostWithRaycastHit(RaycastHit raycast)
        {
            // We successfully hit one of our placement areas
            // Try and get a placement area on the object we hit
            m_CurrentArea = raycast.collider.GetComponent<IPlacementArea>();

            if (m_CurrentArea == null)
            {
                Debug.LogError("There is not an IPlacementArea attached to the collider found on the m_PlacementAreaMask");
                return;
            }
            m_GridPosition = m_CurrentArea.WorldToGrid(raycast.point, m_CurrentBuilding.Building.Dimensions);
            TowerFitStatus fits = m_CurrentArea.Fits(m_GridPosition, m_CurrentBuilding.Building.Dimensions);

            m_CurrentBuilding.Show();
            m_GhostPlacementPossible = fits == TowerFitStatus.Fits;
            m_CurrentBuilding.Move(m_CurrentArea.GridToWorld(m_GridPosition, m_CurrentBuilding.Building.Dimensions),
                                m_CurrentArea.transform.rotation,
                                m_GhostPlacementPossible);
        }
        /// <summary>
		/// The radius of the sphere cast 
		/// for checking ghost placement
		/// </summary>
		public float sphereCastRadius = 1;
        /// <summary>
		/// The physics layer for moving the ghost around the world
		/// when the placement is not valid
		/// </summary>
		public LayerMask ghostWorldPlacementMask;
        /// <summary>
		/// Move ghost with the given ray
		/// </summary>
		protected virtual void MoveGhostOntoWorld(Ray ray, bool hideWhenInvalid)
        {
            m_CurrentArea = null;

            if (!hideWhenInvalid)
            {
                RaycastHit hit;
                // check against all layers that the ghost can be on
                Physics.SphereCast(ray, sphereCastRadius, out hit, 1000f, ghostWorldPlacementMask);
                if (hit.collider == null)
                {
                    return;
                }
                m_CurrentBuilding.Show();
                m_CurrentBuilding.Move(hit.point, hit.collider.transform.rotation, false);
            }
            else
            {
                m_CurrentBuilding.Hide();
            }
        }
    }
}
