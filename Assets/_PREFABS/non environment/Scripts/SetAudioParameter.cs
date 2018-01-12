using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
///	Author: Noah Nam
///	Sets audio volume for a given audio mixer.
 public class SetAudioParameter : MonoBehaviour
 {
     public AudioMixer mixer;
     public string parameterName = "MasterVolume";
     public float Parameter
     {
         get
         {
             float parameter;
             mixer.GetFloat(parameterName, out parameter);
             return parameter;
         }
         set
         {
			 float x=20*Mathf.Log(value,10);
             mixer.SetFloat(parameterName, x);
         }
     }
 }