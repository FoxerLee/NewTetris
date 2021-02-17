using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public List<Obj> objPrefabList;
    public Transform spawnPoint;
    public Button playButton;
    public Text digitsLabel;
    public Text tensLabel;
    public AudioSource collideAudio;
    public AudioSource winAudio;
    public AudioSource loseAudio;
    public int score;
    public Transform camTransform;
    // private CameraShake mainCamera;
    // public int cubeLeft;

    // How long the object should shake for.
	public float shakeDuration = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.1f;
	public float decreaseFactor = 0.5f;

    // control
    private bool isShakeEnabled = true;
    private bool isMusicEnabled = true;
    private bool isRotation = true;
    private bool isExplosion = true;

    private Obj obj;
    private int objId;
    private bool isGameOver;

    // private float originY;
    // private float highestY;

    private List<Obj> objs = new List<Obj>();
    Vector3 originalPos;
	
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}


    // Update is called once per frame
    void Update()
    {
        if(isGameOver)
        {
            return;
        }

        if(score == -1)
        {
            OnGameLose();
        }

        // check whether collapse
        // var tempY = -10f;
        for (int i = 0; i < objs.Count; i++)
        {
            var objPosY = objs[i].rigid.position.y;
            var objPosX = objs[i].rigid.position.x;
            if (objPosY < -6f || objPosX < -3.4f || objPosX > 3.4f) 
            {
                
                // objs[i].Shake();
                

                objs[i].durationTime += Time.deltaTime;
                objs[i].totalTime += Time.deltaTime;
                // Debug.Log(objs[i].durationTime);
                if (isExplosion)
                {
                    if (objs[i].durationTime > objs[i].gapTime)
                    {
                        objs[i].gameObject.SetActive(false);
                        objs[i].durationTime = 0f;
                        // Debug.Log("1");
                    }
                    else
                    {
                        // Debug.Log("2");
                        objs[i].gameObject.SetActive(true);
                        // objs[i].durationTime = 0f;
                    }
                }
                
                
                if (objs[i].totalTime > 1f)
                {

                    if (isShakeEnabled)
                    {
                        shakeDuration = 0.2f;
                    }

                    collideAudio.Play();
                    Destroy(objs[i].gameObject);
                    objs.RemoveAt(i);
                }

                if (shakeAmount < 0.5f)
                {
                    shakeAmount += 0.1f;
                }
                
            }

        }
       
        if (shakeDuration > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shakeDuration = 0f;
			camTransform.localPosition = originalPos;
            
		}

        if (Input.GetMouseButtonUp(0))
        {
            var mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            
            var objPos = new Vector3(worldPos.x, spawnPoint.position.y, -1);
            obj.gameObject.transform.position = objPos;

            obj.SetSimulated(true);

            obj = SpawnNextObj();
        }

        if (Input.GetKeyDown (KeyCode.Q))
        {
            IsShake();
        }

        if (Input.GetKeyDown (KeyCode.W))
        {
            PlayMusic();
        }

        if (Input.GetKeyDown (KeyCode.E))
        {
            TextRotate();
        }

        if (Input.GetKeyDown (KeyCode.R))
        {
            CubeExplosion();
        }
    }

    private Obj SpawnNextObj()
    {
        var prob = Random.Range(0f, 1f);
        var rand = 0;
        if (prob > 0.9)
        {
            rand = Random.Range(objPrefabList.Count-2, objPrefabList.Count);
        }
        else
        {
            rand = Random.Range(0, objPrefabList.Count-2);
        }
        Debug.Log(rand);
        // var rand = Random.Range(0, objPrefabList.Count-2);
        var prefab = objPrefabList[rand].gameObject;
        var pos = spawnPoint.position;
        ChangeScore();

        return SpawnObj(prefab, pos);
    }

    private Obj SpawnObj(GameObject prefab, Vector3 pos)
    {

        var obj = Instantiate(prefab, pos, Quaternion.identity);
        var f = obj.GetComponent<Obj>();
        f.SetSimulated(false);
        f.id = objId++;

        f.OnGameWin = () =>
        {
            if (isGameOver == true)
            {
                return;
            }
            OnGameWin();
        };
        objs.Add(f);



        return f;
    }

    public void Restart()
    {

        for (int i = 0; i < objs.Count; i++)
        {
            objs[i].SetSimulated(false);
            Destroy(objs[i].gameObject);
        }

        objs.Clear();

        playButton.gameObject.SetActive(false);
        obj = SpawnNextObj();
        Debug.Log("Restart");
        score = 50;
        digitsLabel.text = "0";
        tensLabel.text = "5";

        var curTens = tensLabel.transform.position;
        var curDigits = digitsLabel.transform.position;
        tensLabel.transform.position = new Vector3(curTens.x, curDigits.y, curTens.z);
        tensLabel.transform.localRotation = Quaternion.Euler(0, 0, 0);

        isGameOver = false;
    }

    public void IsShake()
    {
        if (isShakeEnabled)
        {
            isShakeEnabled = false;
            var shakeLabel = GameObject.Find("Shake").GetComponent<Text>();
            shakeLabel.color = Color.gray;
        }
        else
        {
            isShakeEnabled = true;
            var shakeLabel = GameObject.Find("Shake").GetComponent<Text>();
            shakeLabel.color = Color.white;
        }
    }

    public void PlayMusic()
    {
        if (isMusicEnabled)
        {
            isMusicEnabled = false;
            var musicLabel = GameObject.Find("Music").GetComponent<Text>();
            musicLabel.color = Color.gray;

            var bgm = GameObject.Find("Ambient").GetComponent<AudioSource>();
            bgm.Pause();

        }
        else
        {
            isMusicEnabled = true;
            var musicLabel = GameObject.Find("Music").GetComponent<Text>();
            musicLabel.color = Color.white;

            var bgm = GameObject.Find("Ambient").GetComponent<AudioSource>();
            bgm.Play();
        }
    }

    public void TextRotate()
    {
        if (isRotation)
        {
            isRotation = false;
            var rotationLabel = GameObject.Find("Rotate").GetComponent<Text>();
            rotationLabel.color = Color.gray;

            var curTens = tensLabel.transform.position;
             var curDigits = digitsLabel.transform.position;
            tensLabel.transform.position = new Vector3(curTens.x, curDigits.y, curTens.z);
            tensLabel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            isRotation = true;
            var rotationLabel = GameObject.Find("Rotate").GetComponent<Text>();
            rotationLabel.color = Color.white;

            var tens = int.Parse(tensLabel.text);

            var curTens = tensLabel.transform.position;
            var curDigits = digitsLabel.transform.position;
            // Debug.Log(curTens);
            tensLabel.transform.position = new Vector3(curTens.x, curDigits.y - (4-tens)*10, curTens.z);
            tensLabel.transform.localRotation = Quaternion.Euler(0, 0, (4-tens)*10f);
        }
    }

    public void CubeExplosion()
    {
        if (isExplosion)
        {
            isExplosion = false;
            var explosionLabel = GameObject.Find("Explosion").GetComponent<Text>();
            explosionLabel.color = Color.gray;

        }
        else
        {
            isExplosion = true;
            var explosionLabel = GameObject.Find("Explosion").GetComponent<Text>();
            explosionLabel.color = Color.white;
        }
    }

    private void OnGameWin()
    {
        isGameOver = true;
        playButton.gameObject.SetActive(true);
        winAudio.Play();
    }

    private void OnGameLose()
    {
        isGameOver = true;
        playButton.gameObject.SetActive(true);
        loseAudio.Play();

    }

    private void RemoveObj(Obj f)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            if (objs[i].id == f.id)
            {
                objs.Remove(f);
                Destroy(f.gameObject);
                return;
            }
        }
    }

    private bool isObjExist(Obj f)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            if (objs[i].id == f.id)
            {
                return true;
            }
        }
        return false;
    }



    private void ChangeScore()
    {
        this.score -= 1;
        int tens = this.score / 10;
        int digits = this.score % 10;
        tensLabel.text = $"{tens.ToString()}";
        digitsLabel.text = $"{digits.ToString()}";


        if (isRotation)
        {
            var curTens = tensLabel.transform.position;
            var curDigits = digitsLabel.transform.position;
            // Debug.Log(curTens);
            tensLabel.transform.position = new Vector3(curTens.x, curDigits.y - (4-tens)*10, curTens.z);
            tensLabel.transform.localRotation = Quaternion.Euler(0, 0, (4-tens)*10f);
        }
        
    }

}
