using Assets.Scripts.Entities.Buildings;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.Placement
{
    internal class BuildingPlaceController : MonoBehaviour
    {
        /// <summary>
		/// True if we're at a valid world position
		/// </summary>
		protected bool m_ValidPos;
        /// <summary>
		/// Target world position
		/// </summary>
		protected Vector3 m_TargetPosition;
        /// <summary>
		/// The list of attached mesh renderers 
		/// </summary>
		protected MeshRenderer[] m_MeshRenderers;

        /// <summary>
		/// The two materials used to represent valid and invalid placement, respectively
		/// </summary>
		public Material material;

        public Material invalidPositionMaterial;
        /// <summary>
		/// Movement velocity for smooth damping
		/// </summary>
		protected Vector3 m_MoveVel;
        /// <summary>
        /// Movement damping factor
        /// </summary>
        public float dampSpeed = 0.075f;

        public Building Building { 
            get { 
                return _building; 
            } 
            set { 
                _building = value;
                m_MeshRenderers = null;

                StartCoroutine(WaitFrameUpdate());
                IEnumerator WaitFrameUpdate()
                {
                    yield return null;
                    m_MeshRenderers = GetComponentsInChildren<MeshRenderer>();
                }
            } 
        }

        private Building _building;



        /// <summary>
        /// Moves this ghost to a given world position
        /// </summary>
        /// <param name="worldPosition">The new position to move to in world space</param>
        /// <param name="rotation">The new rotation to adopt, in world space</param>
        /// <param name="validLocation">Whether or not this position is valid. Ghost may display differently
        /// over invalid locations</param>
        public virtual void Move(Vector3 worldPosition, Quaternion rotation, bool validLocation)
        {
            m_TargetPosition = worldPosition;

            if (!m_ValidPos)
            {
                // Immediately move to the given position
                m_ValidPos = true;
                transform.position = m_TargetPosition;
            }

            transform.rotation = rotation;
            if (m_MeshRenderers == null) return;
            foreach (MeshRenderer meshRenderer in m_MeshRenderers)
            {
                meshRenderer.sharedMaterial = validLocation ? material : invalidPositionMaterial;
            }
        }
        /// <summary>
        /// Show this ghost
        /// </summary>
        public virtual void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                m_MoveVel = Vector3.zero;

                m_ValidPos = false;
            }
        }

        /// <summary>
		/// Hide this ghost
		/// </summary>
		public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
		/// Damp the movement of the ghost
		/// </summary>
		protected virtual void Update()
        {
            Vector3 currentPos = transform.position;

            if (Vector3.SqrMagnitude(currentPos - m_TargetPosition) > 0.01f)
            {
                currentPos = Vector3.SmoothDamp(currentPos, m_TargetPosition, ref m_MoveVel, dampSpeed);

                transform.position = currentPos;
            }
            else
            {
                m_MoveVel = Vector3.zero;
            }
        }

    }
}
