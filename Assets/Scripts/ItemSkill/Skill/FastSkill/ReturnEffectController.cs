using System.Collections;
using UnityEngine;

namespace ItemSkill.Skill.FastSkill
{
    public class ReturnEffectController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer cycloneRenderer;
        [SerializeField] private MeshRenderer godRayRenderer;
        
        private const float TargetDissolve = 0.95f;
        private const float TargetOffset   = 1f;
        
        private float   _duration;
        private Vector3 _position;
        
        private Material _cycloneMaterialInstance;
        private Material _godRayMaterialInstance;

        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");
        private static readonly int Offset = Shader.PropertyToID("_Offset");

        public void Setup(float duration, Vector3 position)
        {
            _duration = duration;
            _position = position;

            _cycloneMaterialInstance = new Material(cycloneRenderer.sharedMaterial);
            _godRayMaterialInstance = new Material(godRayRenderer.sharedMaterial);

            cycloneRenderer.material = _cycloneMaterialInstance;
            godRayRenderer.material = _godRayMaterialInstance;

            _cycloneMaterialInstance.SetFloat(Dissolve, 1);
            _godRayMaterialInstance.SetFloat(Offset, 0);

            // Set position and start effect
            transform.position = position;
            StartCoroutine(ReturnRoutine());
        }
        
        private IEnumerator ReturnRoutine()
        {
            var t = 0f;
            
            while (t < _duration)
            {
                t += Time.deltaTime;
                _cycloneMaterialInstance.SetFloat(Dissolve, Mathf.Lerp(1, TargetDissolve, t / _duration));
                yield return null;
            }
            
            t = 0f;
            var rayDuration = _duration * 0.1f;

            while (t < rayDuration)
            {
                t += Time.deltaTime;
                _godRayMaterialInstance.SetFloat(Offset, Mathf.Lerp(0, TargetOffset, t / rayDuration));
                yield return null;
            }

            t = 0f;
            var disappearDuration = _duration * 0.4f;

            while (t < disappearDuration)
            {
                t += Time.deltaTime;
                _cycloneMaterialInstance.SetFloat(Dissolve, Mathf.Lerp(TargetDissolve, 1, t / disappearDuration));
                yield return null;
            }
            
            Destroy(gameObject);
        }
    }
}
