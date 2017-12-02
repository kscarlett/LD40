using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAnimDebug : MonoBehaviour
{

    private Animator _anim;

	// Use this for initialization
	void Start ()
	{
	    _anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.A))
	    {
	        _anim.SetBool("Attacking", true);
	    } else if (Input.GetKeyDown(KeyCode.W))
	    {
	        _anim.SetBool("Attacking", false);
	    } else if (Input.GetKeyDown(KeyCode.D))
	    {
	        _anim.SetTrigger("Die");
	    }
	}
}
