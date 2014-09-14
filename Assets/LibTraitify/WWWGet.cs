using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WWWGet : MonoBehaviour {
	
	public delegate void WWWDataParseMethod (string s);
	public delegate void WWWDataTexMethod (Texture2D t);
	public delegate void WWWDataTexMethodSerial (Texture2D t,int n,bool flip);
	
	public GameObject panelloading; public Text textloading;

	static string genBasicAuthURL(string authstring,string url){
		return "https://"+authstring + "@" + url;
	}

	public IEnumerator FetchDataAuthPost(string url,WWWDataParseMethod callme,string authstring,string jsonString){
		SetLoadingActive ();
		url = genBasicAuthURL(authstring , url);
		Hashtable headers = new Hashtable();
	
		Hashtable postHeader = new Hashtable();
		headers.Add("Content-Type", "text/json");
		headers.Add("Content-Length", jsonString.Length);
		WWW w = new WWW (url,new System.Text.UTF8Encoding().GetBytes(jsonString),headers);
		yield return w;
		if (w.error == null) {
			callme (w.text);
		} else {
			print (url + " " + w.error);
		}
		SetLoadingInactive ();
	}

	public void FetchDataAuth(string url,WWWDataParseMethod callme,string authstring){
		url = genBasicAuthURL(authstring , url);
		StartCoroutine(FetchData (url, callme));
	}

	public IEnumerator FetchData(string url,WWWDataParseMethod callme){
		SetLoadingActive ();
		WWW w = new WWW (url);
		yield return w;
		if (w.error == null) {
			callme (w.text);
		} else {
			print (url+" " +w.error);
		}
		SetLoadingInactive ();
	}
	
	public IEnumerator FetchDataTexture(string url,WWWDataTexMethod callme){
		SetLoadingActive ();
		WWW w = new WWW (url);
		yield return w;
		if (w.error == null) {
			print ("downloaded...");
			callme (w.texture);
		} else {
			print (url+" " +w.error);
		}
		SetLoadingInactive ();
	}

	public IEnumerator FetchDataTextureSerial(string url,WWWDataTexMethodSerial callme,int n,int total){
		SetLoadingActive ();
		WWW w = new WWW (url);
		yield return w;
		if (w.error == null) {
			print ("downloaded..."+n+" of "+total);
			bool done = false;
			if(n == (total - 1) )
			   done = true;
			callme (w.texture,n,done);
		} else {
			print (url+" " +w.error);
		}
		SetLoadingInactive ();
	}
	
	public void SetLoadingActive(){
		panelloading.SetActive (true); textloading.text = "LOADING...";
	}
	
	public void SetLoadingInactive(){
		panelloading.SetActive (false);
	}
	
}
