/// <summary>
/// 
/// Jack-in-the-box Doll
/// 
/// This is a simple script JUST for showcase purposes (test scene). It coordinates some sounds based on variable iterations and pre-made transitions (animController demo).
/// 
/// NOTE> I do not give support for this script. Feel free to tweak and use it as a base for your own sounds/transitions.
/// 
/// 
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class JackInTheBox : MonoBehaviour {
	
	[SerializeField]AudioClip[] sounds;

	private AudioSource _audioSource;
	private int _iterations = 0;


	void Awake(){

		_audioSource = GetComponent <AudioSource> ();

	}

	// Use this for initialization
	void Start () {

		StartCoroutine (_mecanimSound());

	}
	

	private IEnumerator _mecanimSound(){


		Animator thisAnim = GetComponent<Animator> ();
       
        //anim hashes
        int jackmusic = Animator.StringToHash("Box Handle");
        int laugh01 = Animator.StringToHash("Char - Seek");
        int dslam = Animator.StringToHash("Char / Box - End");


        while (true) {


            switch (_iterations)
            {
                
               

                case (int)iterationsName.jmusic:


                    yield return StartCoroutine(_msc(thisAnim, jackmusic));


                    break;


                case (int)iterationsName.jappearance:


                    if (!GetComponent<AudioSource>().isPlaying)
                    {

                        _audioSource.clip = sounds[1];

                        GetComponent<AudioSource>().Play();

                        yield return new WaitForSeconds(.2f);

                        _audioSource.clip = sounds[2];

                        GetComponent<AudioSource>().Play();

                        yield return StartCoroutine(__nextIteration());


                    }


                    break;

                case (int)iterationsName.laugh01:



                    if (!GetComponent<AudioSource>().isPlaying && thisAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == laugh01) 
                    {

                        _audioSource.clip = sounds[3];

                        yield return new WaitForSeconds(1.2f);

                        GetComponent<AudioSource>().Play();

                        yield return StartCoroutine(__nextIteration());


                    }
                    
                    break;


                case (int)iterationsName.dslam:



                    if (!GetComponent<AudioSource>().isPlaying && thisAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == dslam)
                    {



                        _audioSource.clip = sounds[4];

                        GetComponent<AudioSource>().Play();

                        yield return new WaitForSeconds(.6f);

                        _audioSource.clip = sounds[1];

                        GetComponent<AudioSource>().Play();

                        yield return new WaitForSeconds(.5f);

                        _iterations = 0;



                    }

                    break;



        
            }

			yield return null;
		}

	}



    private IEnumerator _msc(Animator thisAnim, int clip){

        if ((thisAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == clip) && !GetComponent<AudioSource>().isPlaying)
        {

            _audioSource.clip = sounds[_iterations];
            GetComponent<AudioSource>().Play();
        }

        if (thisAnim.GetCurrentAnimatorStateInfo(0).shortNameHash != clip)
         {

             GetComponent<AudioSource>().Stop();

             yield return StartCoroutine(__nextIteration());
         }
    }


    private IEnumerator __nextIteration(){

		++_iterations;
		yield return null;

	}
		

	private enum iterationsName{

		jmusic, jappearance, laugh01, dslam
	}
		

	
}
