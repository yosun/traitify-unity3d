using UnityEngine;
using System.Collections;

public class Traitify : MonoBehaviour {

	public static string APIKEY_public="n4ld6egdmjioi18k1r4jseposu";
	public static string APIKEY_secret="747ke3eeprtq6pu2via2gt9c2d";
	public static string APIURL_base = "api-sandbox.traitify.com/";
	public static string APIURL_version = "v1/";

	public enum ExamState{
		Activated,
		Loading,
		Loaded,
		Started,
		Completed,
		ShowType,
		TypeLoaded,
		TypeDisplayed,
		Deactivated
	}
	public static ExamState es = ExamState.Loading;
	
	public static string assessmentId="";
	public static string[] slidesId;
	public  string[] slidesLandscape;
	public  string[] slidesPortrait;
	public static Texture[] texSlidesLandscape;
	public static Texture[] texSlidesPortrait;
	public static string[] captions;
	int currentSlide = 0;

	public class PersonalityType{
		public string name="";
		public string desc="";
		public string badgeImageURL = "";
		public Texture texImage;
		public string keywords = "";
		public float score = 0f;
	}
	///[System.Serializable]
	public static PersonalityType[] pt = new PersonalityType[7];

	WWWGet wwwget;

	void Awake(){
		wwwget = GetComponent<WWWGet> ();
	}

	void Start(){
		//tests

		// create assessment
		/*CreateAssessment ("hero");
	
		// get slides
		if(assessmentId=="")
			assessmentId = "69774a39-2eef-4599-a410-e4ae7d9e49ac"; // tester
		GetSlides (assessmentId);

		// send response
		SendSlideResponse ("69774a39-2eef-4599-a410-e4ae7d9e49ac","a139140e-46cd-47b6-9315-b64369bf4c3f","true","1200");

		// get personality types
		StartCoroutine(GetPersonalityTypes ("49004517-8c43-48d0-83f9-c2b698b7b6f6"));*/
	}

	public IEnumerator GetPersonalityTypes(string assessmentid){
		string url = GenURLBase ("assessments") + "/" + assessmentid + "/personality_types";
		print ("Getting types for "+assessmentid);
		yield return new WaitForSeconds (1f);
		wwwget.FetchDataAuth(url,PopulatePersonality,GenAuthString());
	}

	public void SendSlideResponse(string assessmentid,string slideid,string response,string timetaken){
		string url = "http://labs.yosun.me/thirdparty/traitify/put.hack.php?assessmentid="+assessmentid+"&slideid="+slideid+"&timetaken="+timetaken+"&response="+response;// GenURLBase("assessments") + "/" + assessmentid + "/slides/"+slideid;
		StartCoroutine(wwwget.FetchData(url,SlideResponseCallback));
	}

	public void CreateAssessment(string type){
		assessmentId = "";
		es = ExamState.Loading;
		if (type == "career") {
			type = "career-deck";
		} else
			type = "super-hero";

		string url = GenURLBase("assessments");
		string jsonstring = "{ \"deck_id\":\""+type+"\" }";
		StartCoroutine(wwwget.FetchDataAuthPost(url,PopulateAssessmentID,GenAuthString(),jsonstring));
	}

	public void GetSlides(string assessmentid){
		string url = GenURLBase ("assessments") + "/" + assessmentid + "/slides";
		wwwget.FetchDataAuth (url,PopulateSlides,GenAuthString());
	}

	void PopulatePersonality(string dump){ print (dump);
		JSONObject json = new JSONObject (dump);
		JSONObject personalitytypes =  json ["personality_types"] ;
		int total = personalitytypes.Count;
		pt = new PersonalityType[7];
		for (int i=0; i<total; i++) {
			pt[i] = new PersonalityType();
			JSONObject eachjson = personalitytypes[i]; 
			print (eachjson["score"]);
			pt[i].score = eachjson["score"].n;
			JSONObject personalitytype = eachjson["personality_type"];
			pt[i].name = personalitytype["name"].str;
			pt[i].desc = personalitytype["description"].str;
			JSONObject badge = personalitytype["badge"];
			pt[i].badgeImageURL = badge["image_large"].str;
			//TODO StartCoroutine(wwwget.FetchDataTexture(pt[i].badgeImageURL,AssignBadgeImage));
			pt[i].keywords = personalitytype["keywords"].str;
		}
		es = ExamState.TypeLoaded;
	}

	void SlideResponseCallback(string dump){//print (dump);
		JSONObject json = new JSONObject (dump);print (json ["completed_at"]);
		if (json ["completed_at"] == null) {
						print ("null");
		} else
						print ("yes");
	}

	void PopulateAssessmentID(string dump){
		print (dump);
		JSONObject json = new JSONObject (dump);
		assessmentId = json ["id"].str;

		GetSlides (assessmentId);
	}

	void PopulateSlides(string slidesdump){
		JSONObject json = new JSONObject (slidesdump);
		int total = 0; total = json.Count;
		slidesLandscape = new string[total];
		slidesPortrait = new string[total];
		slidesId = new string[total];
		texSlidesLandscape = new Texture[total];
		texSlidesPortrait = new Texture[total];
		captions = new string[total];
		for(int i=0;i<total;i++){
			JSONObject eachjson = json[i];
			slidesId[i] = eachjson["id"].str;
			slidesLandscape[i] = eachjson["image_phone_landscape"].str;
			slidesPortrait[i] = eachjson["image_phone_portrait"].str;
			captions[i] = eachjson["caption"].str;
			StartCoroutine(wwwget.FetchDataTextureSerial(slidesLandscape[i],PopulateLandscape,i,total));
			StartCoroutine (wwwget.FetchDataTextureSerial(slidesPortrait[i],PopulatePortrait,i,total));
			currentSlide++;
		}
	}

	void DoneLoadingImages(){
		print ("Done with images! Loaded: "+texSlidesPortrait.Length);
		es = ExamState.Loaded;
	}

	void PopulateLandscape(Texture tex,int n,bool flip){
		texSlidesLandscape [n] = tex;
		if (flip)
			DoneLoadingImages ();
	}
	void PopulatePortrait(Texture tex,int n,bool flip){
		texSlidesPortrait [n] = tex;
		if (flip)
			DoneLoadingImages ();
	}

    static string GenAuthString(){
		return	APIKEY_secret + ":x";
	}

	static string GenURLBase(string endpoint){
		return APIURL_base + APIURL_version + endpoint;
	}

	public static string GetOrientationType(){
		ScreenOrientation so = Screen.orientation;
		if (so == ScreenOrientation.Landscape) {
				return "image_phone_landscape";
		} else {
				return "image_phone_portrait";
		}
	}

}
