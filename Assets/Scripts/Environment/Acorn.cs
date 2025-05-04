using UnityEngine;

namespace Environment
{
    public class Acorn : MonoBehaviour
    {
        private Rigidbody _rb;

		private void Start() 
		{
			_rb = GetComponent<Rigidbody>();
		}

		private void OnCollisionEnter()
		{
			Debug.Log("Collided");
			_rb.isKinematic = true;
		}
    }
}