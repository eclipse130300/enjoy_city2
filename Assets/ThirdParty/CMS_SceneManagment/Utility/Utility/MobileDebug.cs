using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DebugMessageLine{
	private string _message;
	private string _stackTrace;
	private Color _color;
	private int _count;
	private int _maxHeight = 30;
	private DateTime _lastUpdateTime;
	private DateTime _lastChangeTime;
    private float _deltatime;
    private LogType _type;
	public string message{
		get{return _message;}
		set{
			_message=value;
			RefreshTime();
			RefreshChangeTime();
		}
	}
    public DebugMessageLine()
    { }
    public DebugMessageLine(DebugMessageLine line){
	    _message = line._message;
	    _stackTrace = line._stackTrace;
	    _color = line._color;
	    _count = line._count;
	    _maxHeight = line._maxHeight;
	    _lastChangeTime = line._lastChangeTime;
	    _lastUpdateTime = line._lastUpdateTime;
	    _type = line._type;
        _deltatime = line._deltatime;
    }
	public Color color{
		get{return _color;}
		set{
			_color=value;
			RefreshTime();
			//RefreshChangeTime();
		}
	}
	public int maxHeight{
		get{return _maxHeight;}
		set{
			_maxHeight=value;
		}
	}
	public int count{
		get{return _count;}
		set{
			_count=value;
			RefreshTime();
		}
	}
	public LogType type{
		get{return _type;}
		set{
			_type=value;
			RefreshTime();
		}
	}
    public float deltaTime
    {
        get { return _deltatime; }
        set {  _deltatime = value; }
    }
    public DateTime lastUpdateTime{
		get{return _lastUpdateTime;}
	}
	public DateTime lastChangeTime{
		get{return _lastChangeTime;}
	}
	public void RefreshChangeTime(){
		_lastChangeTime =  System.DateTime.Now;
	}
	public void RefreshTime(){
		_lastUpdateTime = System.DateTime.Now;
	}
}
/// <summary>
///  Debug. Консоль для логирования сообщений. Если нет обьекта на сцене, то он создастся автоматически. 
/// </summary>
public class MobileDebug : MonoBehaviour {

	public static MobileDebug instance = null;

	//public GUIStyle lineStyle;
	public  int messageCount = 250;
	public static bool isDebugBuild = false;
	private Dictionary<string,List<DebugMessageLine> > lineDictionary;
	private int showLog = 0;
	private bool pause;
	private string showGroup = null;
	private Vector2 scrollPosition=Vector2.zero;
	private Vector2 scrollGroupPosition=Vector2.zero;
	private float minBtnWidth=0;
	private float minBtnHeight=0;

	public Color colorAsert =  Color.magenta;
	public Color colorError =  Color.red;
	public Color colorException =  Color.cyan;
	public Color colorLog =  Color.gray;
	public Color colorWarning =  Color.yellow;

	public bool enableWorning = true;
	public bool enableAssert = true;
	public bool enableException = true;
	public bool enableError = true;
	public bool enableLog = true;

	//public bool maximize;
	public float cHeight;
	public Rect consoleMaxRect;

    private string inputCommand;

    private GUIStyle lineStyle;
	// Use this for initialization
	void Awake () {
        #if TEST_VERSION
        isDebugBuild = true;
		#else
		isDebugBuild = false;
		#endif
	
		if(instance!=null){
			GameObject.Destroy(this);
		}else{
			#if !UNITY_EDITOR
			Application.logMessageReceived += HandleLog;
			#endif
			instance = this;
		}

/*		DontDestroyOnLoad(this);*/

		if(instance.lineDictionary==null)
			instance.lineDictionary = new Dictionary<string, List<DebugMessageLine> >();	
			
			instance.lineStyle = new GUIStyle();
			

			enableWorning = true;
			enableAssert = true;
			enableException = true;
			enableError = true;
			enableLog = true;
			//maximize = false;
	}
	private static void Init(){
		if(instance==null){
			GameObject obj = Instantiate(new GameObject());
			obj.AddComponent<MobileDebug>();
			instance=obj.GetComponent<MobileDebug>();
			obj.name = "MobileDebug";
			Application.logMessageReceived += HandleLog;
		}

	}
	private Color GetColorFromType(LogType type){
		switch (type){
		    case LogType.Assert:
			    return colorAsert;
		
		    case LogType.Error:
			    return colorError;

		    case LogType.Exception:
			    return colorException;
	
		    case LogType.Log:
			    return colorLog;
	
		    case LogType.Warning:
			    return colorWarning;
		
		    default:
			    return colorLog;
		}
	}
	private static void HandleLog(string logString, string stackTrace, LogType type)
    {

		Log(logString+"\n"+stackTrace,"Unity Log",type);
	}

	/// <summary>
	/// Вывести сообщение в GUI консоль. Создается при первом обращении.
	/// 
	/// </summary>
	/// <param name="message">Сообщение - любой обьект унаследованный от object</param>
	/// <param name="group">Название вкладки в которую выводится сообщение. Создает если ее нет.</param>
	/// <param name="type">Тип сообщение для фильтрации по умолчанию Log</param>
	/// <param name="position">В какой строке консоли вывести сообщение. -1 -выводится на следующей строке</param>
	/// <param name="color">Цвет сообщения в консоле по умолчанию цвет соответствует типу</param>

	public static void Log(object message,string group = "General",LogType type = LogType.Log,int position=-1){
		Log (message,group,type,position,Vector4.zero);
	}
	public static void LogError(object message,string group = "General",LogType type = LogType.Error,int position=-1){
		Log (message,group,type,position,Vector4.zero);
	}
	public static void LogWarning(object message,string group = "General",LogType type = LogType.Warning,int position=-1){
		Log (message,group,type,position,Vector4.zero);
	}
	/// <summary>
	/// Вывести сообщение в GUI консоль. Создается при первом обращении.
	/// 
	/// </summary>
	/// <param name="message">Сообщение - любой обьект унаследованный от object</param>
	/// <param name="group">Название вкладки в которую выводится сообщение. Создает если ее нет</param>
	/// <param name="type">Тип сообщение для фильтрации по умолчанию Log</param>
	/// <param name="position">В какой строке консоли вывести сообщение. -1 -выводится на следующей строке</param>
	/// <param name="color">Цвет сообщения в консоле по умолчанию цвет соответствует типу</param>
	public static void Log(object message,string group,LogType type,int position,Color color){
#if TEST_VERSION
#if UNITY_EDITOR
        if (group.Equals("General")){
			switch(type){
			case LogType.Error:
				Debug.LogError(message);
				break;
			case LogType.Warning:
				Debug.LogWarning(message);
				break;
			default:
				Debug.Log(message);
				break;
			}
			return;
		}

	#endif
	#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID

	#endif
		if(instance==null){
			Init();
		}
		if(color ==(Color) Vector4.zero){
		   color = instance.GetColorFromType(type);
		}

        if (!instance.lineDictionary.ContainsKey(group)){
			instance.lineDictionary.Add(group, new List<DebugMessageLine> ());
			if(instance.showGroup==null){
				instance.showGroup = group;
			}
		}
		if(position!=-1){
			DebugMessageLine newLine = new DebugMessageLine();
			newLine.message = message.ToString();
			newLine.color = color;
			newLine.count = 1;
			newLine.type = type;
            newLine.deltaTime = instance.averageDeltaTime;

            for (int i=instance.lineDictionary[group].Count-1;i<position;i++)
				instance.lineDictionary[group].Add(new DebugMessageLine());
			instance.lineDictionary[group][position]=newLine;
		}else
		if(!instance.ChkEquals(message,color,group)){
			DebugMessageLine newLine = new DebugMessageLine();
			newLine.message = message.ToString();
			newLine.color = color;
			newLine.count = 1;
			newLine.type = type;
            newLine.deltaTime = instance.averageDeltaTime;

            instance.lineDictionary[group].Add(newLine);


			if(instance.lineDictionary[group].Count>instance.messageCount){
				instance.lineDictionary[group].RemoveAt(0);
			}
		}else{

		}

		
	#endif
	}

	private bool ChkEquals(object message,Color color,string group){
		for (int i=0;i<lineDictionary[group].Count;i++ ){
			if(message.ToString()==lineDictionary[group][i].message && color==lineDictionary[group][i].color){
				DebugMessageLine value;
				value = lineDictionary[group][i];
				value.count++;
				lineDictionary[group][i] = value;
				return true;
			}
		}
		return false;
	}
	private bool CompareWisibleStatus(DebugMessageLine line){

		if(enableWorning&&line.type==LogType.Warning)
			return true;
		if(enableAssert&&line.type==LogType.Assert)
			return true;
		if(enableException&&line.type==LogType.Exception)
			return true;
		if(enableError&&line.type==LogType.Error)
			return true;
		if(enableLog&&line.type==LogType.Log)
			return true;
		return false;
	}
	float deltaTime; 
	float averageDeltaTime; 
	void OnGUI(){
        DebugMessageLine[] messageLines = new DebugMessageLine[0]; 

        if (showGroup == null || !instance.lineDictionary.ContainsKey(showGroup))
        {
            showGroup = instance.lineDictionary.Keys.FirstOrDefault();

        }
        if (showGroup != null)
        {
            messageLines = instance.lineDictionary[showGroup].ToArray();
        }

#if TEST_VERSION
        GUI.Label(new Rect(10,Screen.height-60,300,20),"last averageDeltaTime : "+ ((int)(averageDeltaTime*1000f)) / 1000f);
		GUI.Label(new Rect(10,Screen.height-30,300,20),"last deltaTime: "+ ((int)(deltaTime*1000f)) / 1000f);

		minBtnWidth = Screen.width/1024f*120f;
		minBtnHeight =Screen.height/768f*45f;

		//if(maximize)
		//	cHeight = Screen.height;
		//else
		cHeight = Mathf.Clamp(Screen.height/3f * showLog,0, Screen.height- minBtnHeight);
        /*
		if(NGUITools.FindCameraForLayer(8).GetComponent<UICamera>()==null)
			return;
		NGUITools.FindCameraForLayer(8).GetComponent<UICamera>().enabled = !(maximize&&showLog);
        */
		GUILayout.BeginHorizontal();

		if(showLog == 0)
        {
			GUILayout.Space(Screen.width/2-minBtnWidth/2);
		}

		GUILayout.BeginVertical("box");

		if(showLog > 0)
        {
			GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.04f;
			GUI.skin.verticalScrollbarThumb.fixedWidth = GUI.skin.verticalScrollbar.fixedWidth;
			//GUILayout.BeginVertical();
			//GUILayout.BeginArea(new Rect(0,0,Screen.width, Screen.height/3),"Consol","box");
			GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));

			scrollGroupPosition=GUILayout.BeginScrollView (
				scrollGroupPosition,GUILayout.Width(Screen.width/8),GUILayout.Height(cHeight-100));
			GUILayout.BeginVertical();
				foreach(KeyValuePair<string,List<DebugMessageLine> >pair in instance.lineDictionary){	
				    if(GUILayout.Button(pair.Key,GUILayout.Width(minBtnWidth),GUILayout.Height(minBtnHeight)))
                    {
                        showGroup = pair.Key;
					}
				}
			GUILayout.EndVertical();

			GUILayout.EndScrollView();
    
			GUILayout.FlexibleSpace();

			scrollPosition=GUILayout.BeginScrollView (
				scrollPosition,GUILayout.Width(Screen.width- Screen.width/8),GUILayout.MinHeight(cHeight-100));

			foreach(DebugMessageLine  line in messageLines)
            {
				if(CompareWisibleStatus(line))
                {

					lineStyle.normal.textColor=line.color;
					lineStyle.fontSize = Screen.width/1024*14;
					lineStyle.wordWrap = true;
                    
					GUILayout.BeginHorizontal("box");

					if(GUILayout.Button (string.Format("{0}",line.message),lineStyle,GUILayout.Width(Screen.width- Screen.width/4.1f),GUILayout.Height(line.maxHeight))){
                        
                        SetMaxWidth(line);
						//MobileDebug.Log(line.maxHeight);
					}
					lineStyle.fontSize = Screen.width/1024*11;
					GUILayout.Label(string.Format("count:{0}\nlastUpdate:\n{1} \nlastChange:\n{2}\ndeltatime:\n{3}", line.count.ToString(),line.lastUpdateTime.ToString("hh:mm:ss:ms"),line.lastChangeTime.ToString("hh:mm:ss:ms"), line.deltaTime),lineStyle,GUILayout.MinWidth(70));
								
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

        }

		lineStyle.normal.textColor=Color.white;
		if(showLog>0)
        {
			GUILayout.BeginHorizontal();

			if(GUILayout.Button(enableAssert?"Assert *":"Assert",GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight))){
				enableAssert= !enableAssert;
			}
			if(GUILayout.Button(enableError?"Error *":"Error",GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight))){
				enableError = !enableError;
			}
			if(GUILayout.Button(enableException?"Exception *":"Exception",GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight))){
				enableException = !enableException;
			}
			if(GUILayout.Button(enableWorning?"Warning *":"Warning",GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight))){
				enableWorning = !enableWorning;
			}
			if(GUILayout.Button(enableLog?"Log *":"Log",GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight))){
				enableLog = !enableLog;
			}
			GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear", GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight)))
            {
                ClearLog(showGroup);
            }
            if (GUILayout.Button("ClearAll",GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight)))
            {
				ClearAllLog();
			}

            GUILayout.Space(10);
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            inputCommand = GUILayout.TextField(inputCommand, GUILayout.MinHeight(minBtnHeight / 2));
            if (GUILayout.Button("►",GUILayout.MaxWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight/2)))
            {
                InvokeCommand(inputCommand);
            }
            GUILayout.EndHorizontal();
        }
        //inputCommand = GUILayout.TextField(inputCommand);

        if (showLog == 0)
        {
            if (GUILayout.Button("Console", GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight)))
            {
                showLog++;
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("▲", GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight)))
            {
                showLog--;
                if (showLog > 3)
                {
                    showLog = 0;
                }

            }
            if (GUILayout.Button("▼", GUILayout.MinWidth(minBtnWidth), GUILayout.MinHeight(minBtnHeight)))
            {
                showLog++;
                if (showLog > 3)
                {
                    showLog = 0;
                }

            }
            GUILayout.EndHorizontal();
        }
        

		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	#endif
	}
	public void SetMaxWidth(DebugMessageLine line){

		if(line.maxHeight == 0)
            line.maxHeight = 30;

        else if (line.maxHeight == 30)
            line.maxHeight = 250;
        else if (line.maxHeight == 250)
            line.maxHeight = 450;
        else
            line.maxHeight = 30;
        
	}
	public void ClearAllLog(){
		instance.lineDictionary.Clear();
	}
	private void ClearLog(string group){
		instance.lineDictionary.Remove(group); 
	}
	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width*height];
		
		for(int i = 0; i < pix.Length; i++){
			pix[i] = col;
		}

		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		
		return result;
	}
	void OnDestroy(){
		#if !UNITY_EDITOR
		Application.logMessageReceived -= HandleLog;
		#endif
//		instance = null;
//		try{
//			GameObject.Destroy(this);
//			GameObject.Destroy(gameObject);
//		}catch{
//			GameObject.DestroyImmediate(this);
//			GameObject.DestroyImmediate(gameObject);
//		}

	}



	float tmpTime;
	// Update is called once per frame
	void Update () {
		if(tmpTime == 0){
			tmpTime  =  Time.deltaTime;
		} else{
			tmpTime = (tmpTime+ Time.deltaTime)/2;
		}
		if(Time.frameCount % 15 == 0){
			deltaTime = tmpTime;
			tmpTime = 0;
			if(averageDeltaTime == 0){
				averageDeltaTime  =  deltaTime;
			} else{
				averageDeltaTime = (averageDeltaTime+deltaTime)/2;
			}
		}
	
	}


    #region console
    private void InvokeCommand(string command)
    {
        Log(command, "Commands");
        inputCommand = "";
    }
    #endregion
}
