using System;
using System.Collections;
using System.Collections.Generic;
using CartoonFX;
using UnityEngine;
using UnityEngine.Events;



public class ObjectDestroy : MonoBehaviour
{
        public ParticleSystem destroyEffect;
        
        //public static bool nextStage = false;
        public int findCount = 0;
    
        public UnityEvent Interacted { get; set; }

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayDestory();
            }

            if (findCount == 5)
            {
                //nextStage = true;
                // 포탈 활성화
            }
        }

        void PlayDestory()
        {
            if (destroyEffect == null)
            {
                Debug.Log("destroyEffect 없음");
                Destroy(gameObject);
                return;
            }
            
            ParticleSystem effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(gameObject);

        }

        /// <summary>
        /// 플레이어가 오브젝트에 상호작용 시도 시 호출하는 메서드
        /// </summary>
        public void Interact()
        {
            Interacted?.Invoke();
            // 오브젝트 파괴
            //GameObject.Destroy(gameObject);
            //findCount++;
        }

        /// <summary>
        /// 플레이어가 해당 오브젝트를 주시하고 있을 때(타깃 설정) 호출될 메소드 (선택)
        /// </summary>
        public void OnInteractFocusEnter()
        {
            Interact();
        }

        /// <summary>
        /// 플레이어가 해당 오브젝트를 더 이상 주시하지 않을 때(타깃 X) 호출될 메소드 (선택)
        /// </summary>
        public void OnInteractFocusExit()
        {
            
        }
}
