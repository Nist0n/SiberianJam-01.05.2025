﻿using Settings.Audio;
using UnityEngine;

namespace Runner
{
    public class LevelGenerator : MonoBehaviour
    {
        public GameObject Tile1;
        public GameObject Tile2;
        public GameObject StartTile;

        private float _timer;
        private float _timeToVoice;

        private float Index = 0;

        private void Start()
        {
            SetRandomTime();
            AudioManager.instance.PlayAmbient("Car loop");
            //create 5 tile first game
            GameObject StartPlane1 = Instantiate(StartTile, transform);
            StartPlane1.transform.position = new Vector3(7, 0, 0);
        
            GameObject StartPlane2 = Instantiate(StartTile, transform);
            StartPlane2.transform.position = new Vector3(-1, 0, 0);
       
            GameObject StartPlane3 = Instantiate(StartTile, transform);
            StartPlane3.transform.position = new Vector3(-9, 0, 0);
       
            GameObject StartPlane4 = Instantiate(StartTile, transform);
            StartPlane4.transform.position = new Vector3(-17, 0, 0);
       
            GameObject StartPlane5 = Instantiate(StartTile, transform);
            StartPlane5.transform.position = new Vector3(-25, 0, 0);
        }

        private void Update()
        {
            gameObject.transform.position += new Vector3(4 * Time.deltaTime, 0, 0);  //tiles movment X direction axis

            if(transform.position.x >= Index)
            {
                int RandomInt1 = Random.Range(0, 2);

                if(RandomInt1 == 1)
                {
                    GameObject TempTile1 = Instantiate(Tile1, transform);
                    TempTile1.transform.position = new Vector3(-16, 0, 0);
                }
                else if(RandomInt1 == 0)
                {
                    GameObject TempTile1 = Instantiate(Tile2, transform);
                    TempTile1.transform.position = new Vector3(-16, 0, 0);
                }

                int RandomInt2 = Random.Range(0, 2);

                if(RandomInt2 == 1)
                {
                    GameObject TempTile2 = Instantiate(Tile1, transform);
                    TempTile2.transform.position = new Vector3(-24, 0, 0);
                }
                else if(RandomInt2 == 0)
                {
                    GameObject TempTile2 = Instantiate(Tile2, transform);
                    TempTile2.transform.position = new Vector3(-24, 0, 0);
                }

                Index = Index + 15.95f;
            }

            if (_timeToVoice >= _timer)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                _timer = 0;
                SetRandomTime();
                AudioManager.instance.PlayRandomVoiceLine(1);
            }
        }

        private void SetRandomTime()
        {
            _timeToVoice = Random.Range(10, 20);
        }
    }
}
