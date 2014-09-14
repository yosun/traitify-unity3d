using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TraitifyUI : MonoBehaviour {

	public GameObject panelTraitify;
	public GameObject panelControllable;
	public GameObject panelShowType; public RawImage imgCointype; public Text txtTypeName; public Text txtTypeDesc;
	public RawImage ri;
	public Text caption;

	public Traitify traitify;

	public static int currentSlide = -1 ;
	static float timeElapsed = 1f;

	void Start () {
		LoadTest ();
	}

	void Update () {
		if (Traitify.ExamState.Started == Traitify.es)
			timeElapsed += Time.deltaTime;
		else if (Traitify.es == Traitify.ExamState.Loaded) {
			StartTest ();
		} else if (Traitify.es == Traitify.ExamState.Completed) {
			ShowType ();
		} else if (Traitify.es == Traitify.ExamState.TypeLoaded) {
			ShowTypeOnscreen();
		}
	}

	void ShowType(){
		Traitify.es = Traitify.ExamState.ShowType;

		StartCoroutine(traitify.GetPersonalityTypes (Traitify.assessmentId));
	}

	void ShowTypeOnscreen(){
		Traitify.es = Traitify.ExamState.TypeDisplayed;

		panelShowType.SetActive (true);
		txtTypeDesc.text = Traitify.pt [0].desc;
		txtTypeName.text = Traitify.pt [0].name;
		// TODO type image
	}

	public void ActivateUI(){
		Traitify.es = Traitify.ExamState.Activated;
		panelTraitify.SetActive (true);
	}

	public void DeactivateUI(){
		Traitify.es = Traitify.ExamState.Deactivated;
		panelTraitify.SetActive (false);
	}

	public void LoadTest(){
		traitify.CreateAssessment ("hero");

	}

	public void AssignPersonality(){
		//TODO 
		print ("YOU are a "+Traitify.pt[0].name);
	}

	public void StartTest(){
		panelControllable.SetActive (true);
		panelShowType.SetActive (false);
		Traitify.es = Traitify.ExamState.Started;
		currentSlide = -1;

		GoNextSlide (currentSlide);
	}

	public void EndTest(){
		Traitify.es = Traitify.ExamState.Completed;
		panelControllable.SetActive (false);
		panelShowType.SetActive (true);
	}

	public void SelectFalse(){
		if (Traitify.es == Traitify.ExamState.Started) {
			print (currentSlide + " false");
			traitify.SendSlideResponse (Traitify.assessmentId, Traitify.slidesId [currentSlide], "false", Mathf.RoundToInt (timeElapsed).ToString ());

			GoNextSlide (currentSlide);
		}
	}

	public void SelectTrue(){
		if (Traitify.es == Traitify.ExamState.Started) {
			print (currentSlide + " true");
			traitify.SendSlideResponse (Traitify.assessmentId, Traitify.slidesId [currentSlide], "true", Mathf.RoundToInt (timeElapsed).ToString ());

			GoNextSlide (currentSlide);
		}
	}

	void GoNextSlide(int slide){
		currentSlide++;

		if (currentSlide < Traitify.texSlidesPortrait.Length) {
			PlasterSlide (currentSlide);
			timeElapsed = 1f;
		}else {
			EndTest();
		}
	}

	void PlasterSlide(int i){
		print ("Slide "+i);
		ScreenOrientation so = Screen.orientation;
		if (so == ScreenOrientation.Portrait)
			ri.texture = Traitify.texSlidesPortrait [i];
		else 
			ri.texture = Traitify.texSlidesLandscape [i];
		caption.text = Traitify.captions [i];
	}

}
