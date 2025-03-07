using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace Blind
{
    public class WaveBullet : MonoBehaviour
    {
        [SerializeField] private AnimationCurve spreadAnimation;
        [SerializeField] private float maxsize = 30f;
        [SerializeField] private float time;
        [SerializeField] private float _coolTime;

        private Light2D _light2D;

        private Rigidbody2D rigid;
        private bool facing;
        [SerializeField] private int speed;
        private bool isFire = false;
        private GameObject enemy;
        private bool isEnemyCheck = false;
        private Vector2 position;
        private bool isExit;
        public void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            _light2D = GetComponent<Light2D>();
        }

        public void GetFacing(bool _playerfacing)
        {
            facing = _playerfacing;
            isFire = true;
        }

        public void FixedUpdate()
        {
            if (isFire)
            {
                if (facing) rigid.velocity = transform.right * speed;
                else rigid.velocity = -transform.right * speed;
            }
            else
            {
                if (isEnemyCheck)
                {
                    transform.position = enemy.transform.position;
                }
                else
                {
                    gameObject.transform.position = position;
                }
            }
        }

        public void Bright()
        {
            StartCoroutine(_Bright());
            StartCoroutine(_ExitBright());
        }

        public IEnumerator _Bright()
        {
            var curTime = 0f;
            var radius = 0f;
            while (radius<maxsize)
            {
                curTime += Time.deltaTime;
                radius +=spreadAnimation.Evaluate(curTime / time) * maxsize;
                _light2D.pointLightOuterRadius = radius;
                yield return null;
            }
        }

        public IEnumerator _ExitBright()
        {
            yield return new WaitForSeconds(_coolTime);
            
            var curTime = 0f;
            var radius = maxsize;
            while (radius > 0)
            {
                curTime += Time.deltaTime;
                radius -= spreadAnimation.Evaluate(curTime / time) * maxsize;
                _light2D.pointLightOuterRadius = radius;
                yield return null;
            }
            Destroy(gameObject);
        }

        public void ActivateInvisibleFloor()
        {
            GameObject[] floors = GameObject.FindGameObjectsWithTag("InvisibleFloor");
            for (int i = 0; i < floors.Length; i++)
            {
                floors[i].GetComponent<InvisibleFloor>().SetVisible();
            }
        }
        public void OnTriggerEnter2D(Collider2D col)
        {
            if (isExit) return;
            
            if (col.gameObject.layer == 6 || col.gameObject.layer == 7)
            {
                Bright();
                ActivateInvisibleFloor();
                position = gameObject.transform.position;
                isFire = false;
                isExit = true;
            }
            else if (col.gameObject.layer == 14)
            {
                Bright();
                ActivateInvisibleFloor();
                enemy = col.gameObject;
                isEnemyCheck = true;
                isFire = false;
                isExit = true;
            }
        }
    }
}