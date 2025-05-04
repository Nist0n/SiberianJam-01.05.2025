using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{
    [SerializeField] private Animator egg;
    [SerializeField] private Animator needle;
    [SerializeField] private GameObject cutscene;
    

    private void Start()
    {
        StartCoroutine(Crash());
    }

    private IEnumerator Crash()
    {
        yield return new WaitForSeconds(2);
        egg.Play("CrashEgg");
        yield return new WaitForSeconds(2);
        needle.Play("Crash");
        yield return new WaitForSeconds(2);
        cutscene.SetActive(true);
    }
}
