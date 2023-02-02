using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //public static GameController instance;
    [HideInInspector]
    public int SelectedVehicle;
    public WheelDrive[] AvailableVehicles;
    public GameObject[] RefillList0, RefillList1, RefillList2, RefillList3, RefillList4;
    public Transform AvailablePlayerVehicle;
    public VariableJoystick JoystickUsed;
    //List<Transform> DeliveryPoints;
    Transform CurrentDeliveryPt,CurrentCollectingPt;
    public Transform TutorialDeliveryPt;
    public Transform StoreLocation;
    public Transform DirectionArrow;
    public GameObject[] CamFoodStoreList, StoreHotSpotList;
    public GameObject GameplayCam,StoreHotSpot;
    public Animator[] StoreAnimatorList;
    public Animator StoreAnimator;
    public GameObject[] PoorCharacter,DonatingCharacter;
    public GameObject CoinPrefab, TimerPrefab, RepairPrefab;
    [HideInInspector]
    public int Distance;
    private int NearestStoreIndex;
    [Header("Player")]
    public float HitTakenValue = 5;

    [Header("XP Related")]
    public string[] XPNames;
    public float[] XPMaxValues;

    [Header("Traffic")]
    public GameObject[] traficVehiclesPrefabList;
    public GameObject[] CrowdPrefabList;
    public GameObject StoreRender;

    public bool vehicleWthWhlCldrs;

    [Header("LowEndDeviceCheckArea")]
    public GameObject PostProcessing;
    private void Awake()
    {
        if (SystemInfo.systemMemorySize < 3072)//3gb
            PostProcessing.SetActive(false);

        GameManager.Instance.TraficVehiclesPrefbList = traficVehiclesPrefabList;

        //PlayerPrefs.SetInt("XPLvl", 0);//for test mts10

        if(PlayerPrefs.GetInt("Environment")==1)
        {
            GameManager.Instance.FoodStoreSelected = 0;
        }
        else
        {
            if (PlayerPrefs.GetInt("XPLvl") >= 8)
            {
                GameManager.Instance.FoodStoreSelected = 4;
            }
            else if (PlayerPrefs.GetInt("XPLvl") >= 6)
            {
                GameManager.Instance.FoodStoreSelected = 3;
            }
            else if (PlayerPrefs.GetInt("XPLvl") >= 4)
            {
                GameManager.Instance.FoodStoreSelected = 2;
            }
            else if (PlayerPrefs.GetInt("XPLvl") >= 2)
            {
                GameManager.Instance.FoodStoreSelected = 1;
            }
            else
            {
                GameManager.Instance.FoodStoreSelected = 0;
            }
        }
        
        CamFoodStoreList[GameManager.Instance.FoodStoreSelected].SetActive(true);
        
    }
    void Start()
    {
        //instance = this;
        //PlayerPrefs.DeleteAll();//for test
        //GameManager.Instance.ModeSelected = 1;//for test careermode

        PlayerPrefs.SetInt("FirstStart", 0);
        GameManager.Instance.GameControllerInst = this;
        SelectedVehicle = PlayerPrefs.GetInt("SelectedVehicle");
        if(RefillList0.Length>SelectedVehicle)
            RefillList0[SelectedVehicle].SetActive(true);
        if (RefillList1.Length > SelectedVehicle)
            RefillList1[SelectedVehicle].SetActive(true);
        if (RefillList2.Length > SelectedVehicle)
            RefillList2[SelectedVehicle].SetActive(true);
        if (RefillList3.Length > SelectedVehicle)
            RefillList3[SelectedVehicle].SetActive(true);
        if (RefillList4.Length > SelectedVehicle)
            RefillList4[SelectedVehicle].SetActive(true);
        GameManager.Instance.CurrentVehicle = AvailableVehicles[SelectedVehicle];//for wheeldrive
        GameManager.Instance.CurrentVehicle.transform.position = StoreHotSpotList[GameManager.Instance.FoodStoreSelected].transform.position + new Vector3(0, 0.65f, 0);
        GameManager.Instance.CurrntVehicleContrlr = GameManager.Instance.CurrentVehicle.GetComponent<InGameVehicleContrlr>();
        GameManager.Instance.PlayerVehicle = AvailablePlayerVehicle;//for kartController
        GameManager.Instance.joyStickUsed = JoystickUsed;
        Debug.Log("ssssssss");
        //DeliveryPoints = new List<Transform>();
        
        GameManager.Instance.PoorCharactersInstList = new List<GameObject>();
        GameManager.Instance.DonatingCharacterList = new List<GameObject>();
        //        GameObject[] deliveryPtObjcts = GameObject.FindGameObjectsWithTag("DLPoint");
        //        for(int i=0;i< deliveryPtObjcts.Length;i++)
        //        {
        //                DeliveryPoints.Add(deliveryPtObjcts[i].transform);
        ////            Debug.Log(deliveryPtObjcts[i].transform.position + "dlpts");
        //        }
        //selectDeliveryPt();
        NodeController.NodeCntrlrInst.FoodStoreNodes[GameManager.Instance.FoodStoreSelected].SetActive(true);
        StartRefillAnim();
        if (PlayerPrefs.GetInt("TutTimer") == 0)
            GameManager.Instance.DriveStart = true;
        else
            GameManager.Instance.DriveStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(CurrentDeliveryPt!=null)
        {
            //GPS Direction Calculation
            Vector3 direction = Vector3.zero;
            if (vehicleWthWhlCldrs)
            {
                if (GameManager.Instance.UIControllerInst.PlayerFoodSlider.value <= 0)
                {
                    direction = GameManager.Instance.UIControllerInst.StoreMiniMapSpotList[NearestStoreIndex].transform.position - GameManager.Instance.CurrentVehicle.transform.position;//for wheeldrive
                    //direction = GameManager.Instance.UIControllerInst.StoreMiniMapSpotList[GameManager.Instance.FoodStoreSelected].transform.position - GameManager.Instance.CurrentVehicle.transform.position;//for wheeldrive
                }
                else
                direction = CurrentDeliveryPt.position - GameManager.Instance.CurrentVehicle.transform.position;//for wheeldrive
            }   
            else
                direction = CurrentDeliveryPt.position - GameManager.Instance.PlayerVehicle.transform.position;//for kartController

            Distance = Mathf.RoundToInt(direction.magnitude / 5);//distance calculation

            //Tech 1
            direction = direction.normalized;
            Quaternion DirRotation = Quaternion.LookRotation(direction);
            DirectionArrow.transform.rotation = DirRotation;

            //Tech 2

            //float angleRot = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            //print(angleRot);
            //DirectionArrow.transform.eulerAngles = new Vector3(55, 0, (angleRot));

            //Distance Calculation
            GameManager.Instance.UIControllerInst.DistanceText.text = Distance.ToString() + " M";
            //next challenge
            GameManager.Instance.UIControllerInst.ChallengeDistText.text = Distance+" Meter Away";
            if(GameManager.Instance.TimeToDeliverSeconds<=0)
                GameManager.Instance.UIControllerInst.ChallengeTimeText.text = "No Time Limit";
            else
            {
                float minutes = Mathf.Floor(GameManager.Instance.TimeToDeliverSeconds / 60);
                float seconds = Mathf.RoundToInt(GameManager.Instance.TimeToDeliverSeconds % 60);
                GameManager.Instance.UIControllerInst.ChallengeTimeText.text = minutes.ToString("00") + " Min " + seconds.ToString("00")+" Sec";
                if (minutes <= 0)
                    GameManager.Instance.UIControllerInst.ChallengeTimeText.text = seconds + " Sec";
                if (seconds <= 0)
                    GameManager.Instance.UIControllerInst.ChallengeTimeText.text = minutes + " Min";
            }
                //GameManager.Instance.UIControllerInst.ChallengeTimeText.text = GameManager.Instance.TimeToDeliverSeconds+" Seconds";


            //Timer Calculation
            if (GameManager.Instance.ModeSelected == 0)
            {
                if (!GameManager.Instance.SelectedChallenge.TimeChallenge)
                {
                    GameManager.Instance.UIControllerInst.TimerAnimator.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    TimerCalculation();
                }
                
            }
            else
            {
                TimerCalculation();
            }
            

        }
    }

    private int GetNearestFoodStore()
    {
        int shortestIndex = 0;
        Vector3 direction;
        float shortDist=0;
        for(int i=0;i< GameManager.Instance.UIControllerInst.StoreMiniMapSpotList.Length;i++)
        {
            if(GameManager.Instance.UIControllerInst.StoreMiniMapSpotList[i].transform.root.gameObject.activeSelf)
            {
                direction = GameManager.Instance.UIControllerInst.StoreMiniMapSpotList[i].transform.position - GameManager.Instance.CurrentVehicle.transform.position;
                if (shortDist == 0)
                {
                    shortDist = direction.magnitude;
                    shortestIndex = i;
                }
                else if (direction.magnitude < shortDist)
                {
                    shortDist = direction.magnitude;
                    shortestIndex = i;
                }
            } 
        }
        return shortestIndex;
    }

    private void TimerCalculation()
    {
        if(GameManager.Instance.GameOver)
        return;
        float minutes = Mathf.Floor(GameManager.Instance.TimeToDeliverSeconds / 60);
        float seconds = Mathf.RoundToInt(GameManager.Instance.TimeToDeliverSeconds % 60);
        GameManager.Instance.UIControllerInst.TimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        //if (PlayerPrefs.GetInt("TutDelivery") == 0 || !GameManager.Instance.DriveStart)
        //    return;
        if (GameManager.Instance.DriveStart)
        {
            if (PlayerPrefs.GetInt("TutDelivery") == 0)
                return;
        }
        else
            return;

        if (GameManager.Instance.TimeToDeliverSeconds > 0 && !GameManager.Instance.GameOver)
        {
            if (GameManager.Instance.TimeToDeliverSeconds < 15)
            {
                if (!GameManager.Instance.UIControllerInst.TimeRunOutEfx.activeSelf)
                {
                    GameManager.Instance.UIControllerInst.TimeRunOutEfx.SetActive(true);
                    GameManager.Instance.UIControllerInst.TimerAnimator.SetTrigger("timeOut");
                    GameManager.Instance.UIControllerInst.TimeRunOutEfx2.SetActive(true);
                }
                if (GameManager.Instance.TimeToDeliverSeconds <= 10)
                {
                    if (!GameManager.Instance.UIControllerInst.TimeTickRiser.isPlaying)
                        GameManager.Instance.UIControllerInst.TimeTickRiser.Play();
                }
            }
            else
            {
                GameManager.Instance.UIControllerInst.TimeTickRiser.Stop();
                GameManager.Instance.UIControllerInst.TimeRunOutEfx.SetActive(false);
                GameManager.Instance.UIControllerInst.TimeRunOutEfx2.SetActive(false);
            }


            GameManager.Instance.TimeToDeliverSeconds -= Time.deltaTime;
            //float minutes = Mathf.Floor(GameManager.Instance.TimeToDeliverSeconds / 60);
            //float seconds = Mathf.RoundToInt(GameManager.Instance.TimeToDeliverSeconds % 60);
            //GameManager.Instance.UIControllerInst.TimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else if (!GameManager.Instance.GameOver)
        {
            print("check_timeend1");
            GameManager.Instance.UIControllerInst.TimeText.text = "00:00";
            //GameManager.Instance.UIControllerInst.CallGameOver(1);
            if (GameManager.Instance.ModeSelected == 0)
            {
                GameManager.Instance.UIControllerInst.CallChallengeEnd(1);
            }
            else
            {
                print("check_timeend2");
                if (!GameManager.Instance.UIControllerInst.RevivePopUp.activeSelf)
                    StartCoroutine(delayForCallRevive());
            }              
        }
    }
    IEnumerator delayForCallRevive()
    {
        print("check_delaycallrevive1");
        GameManager.Instance.UIControllerInst.ReviveTimeBool = true;
        yield return new WaitForSeconds(1);
        if (!GameManager.Instance.UIControllerInst.RevivePopUp.activeSelf)
        {
            print("check_delaycallrevive2");
            PlayerPrefs.SetInt("GmOvrType", 0);//0 timeover 1 explode
            GameManager.Instance.UIControllerInst.ReviveHeadingText.text = "Clock is ticking";
            GameManager.Instance.UIControllerInst.ReviveDescripText.text = "Somebody still has an empty stomach";
            GameManager.Instance.UIControllerInst.CallRevive();
        }
    }
    void selectDeliveryPt()
    {
        GameManager.Instance.deliveryPtObjcts = ShuffleList(GameManager.Instance.deliveryPtObjcts);
        //if (CurrentDeliveryPt != null)
        //    GameManager.Instance.deliveryPtObjcts.Add(CurrentDeliveryPt);

        //difficulty progression
        bool processBool = true;
        float timeToDeliver = 0;

        if(PlayerPrefs.GetInt("TutDelivery")==0)
        {
            for (int i = 0; i < GameManager.Instance.deliveryPtObjcts.Count; i++)
            {
                if (GameManager.Instance.deliveryPtObjcts[i].name == "1_Tut")
                {

                    // Yes
                    CurrentDeliveryPt = GameManager.Instance.deliveryPtObjcts[i];
                    GameManager.Instance.deliveryPtObjcts.RemoveAt(i);
                    break; //don't need to check the remaining ones now that we found one
                }
            }


            //poor guy generation
            if (GameManager.Instance.PoorCharactersInstList.Count > 1)
            {
                print("poorCount_" + GameManager.Instance.PoorCharactersInstList.Count);
                Destroy(GameManager.Instance.PoorCharactersInstList[0]);
                GameManager.Instance.PoorCharactersInstList.RemoveAt(0);
                print("poorCount2_" + GameManager.Instance.PoorCharactersInstList.Count);
            }
            GameObject poorInst = Instantiate(PoorCharacter[Random.Range(0, PoorCharacter.Length)], CurrentDeliveryPt.position, CurrentDeliveryPt.transform.rotation);//Random.Range(0,360)Quaternion.Euler(0,0,0)
            GameManager.Instance.PoorCharactersInstList.Add(poorInst);
            GameManager.Instance.NextPoorGuy = poorInst.GetComponent<PoorGuyScript>();

        }
        else
        {
            int stepValue = 0;//for challengemode
            while (processBool)
            {
                
                if (CurrentDeliveryPt != null)
                    GameManager.Instance.deliveryPtObjcts.Add(CurrentDeliveryPt);
                CurrentDeliveryPt = GameManager.Instance.deliveryPtObjcts[0];
                GameManager.Instance.deliveryPtObjcts.RemoveAt(0);
                Vector3 direction = Vector3.zero;
                if (vehicleWthWhlCldrs)
                    direction = CurrentDeliveryPt.position - GameManager.Instance.CurrentVehicle.transform.position;//for wheeldrive
                else
                    direction = CurrentDeliveryPt.position - GameManager.Instance.PlayerVehicle.transform.position;//for kartController
                Distance = Mathf.RoundToInt(direction.magnitude / 5);

                if (GameManager.Instance.ModeSelected == 0)//challenge mode
                {
                    if (Distance > GameManager.Instance.SelectedChallenge.MinDeliveryDist && Distance<(GameManager.Instance.SelectedChallenge.MaxDeliveryDist+stepValue))
                    {
                        processBool = false;
                        timeToDeliver = Distance + 15;
                    }
                    stepValue += 2;
                }
                else
                {
                    if (GameManager.Instance.DeliveryCount < 2)
                    {
                        if (Distance < 50)//Distance < 50 mts10
                        {
                            processBool = false;
                            timeToDeliver = Distance + 15;
                        }

                    }
                    else if (GameManager.Instance.DeliveryCount < 4)
                    {
                        if (Distance < 100 && Distance > 50)
                        {
                            processBool = false;
                            timeToDeliver = Distance + 25;
                        }
                    }
                    else if (GameManager.Instance.DeliveryCount < 6)
                    {
                        if (Distance < 150 && Distance > 100)
                        {
                            processBool = false;
                            timeToDeliver = Distance + 40;
                        }
                    }
                    else
                    {
                        processBool = false;
                        timeToDeliver = Distance;
                    }
                }
            }
            //timeToDeliver = 200;//for test mts10
            //poor guy generation
            if (GameManager.Instance.PoorCharactersInstList.Count > 1)
            {
                print("poorCount_" + GameManager.Instance.PoorCharactersInstList.Count);
                Destroy(GameManager.Instance.PoorCharactersInstList[0]);
                GameManager.Instance.PoorCharactersInstList.RemoveAt(0);
                print("poorCount2_" + GameManager.Instance.PoorCharactersInstList.Count);
            }
            GameObject poorInst = Instantiate(PoorCharacter[Random.Range(0, PoorCharacter.Length)], CurrentDeliveryPt.position, CurrentDeliveryPt.transform.rotation);//Random.Range(0,360)Quaternion.Euler(0,0,0)
            GameManager.Instance.PoorCharactersInstList.Add(poorInst);
            GameManager.Instance.NextPoorGuy = poorInst.GetComponent<PoorGuyScript>();
            print("nextbug000");
            if (PlayerPrefs.GetInt("TutTimer")==1)
            {
                print("nextbug111");
                //if (GameManager.Instance.DeliveryCount == 0)
                //    GameManager.Instance.UIControllerInst.TutorialClicks(2);
                //else
                if (GameManager.Instance.ModeSelected == 0 && GameManager.Instance.UIControllerInst.PlayerFoodSlider.value <= 0)//challenge mode
                {
                    StartCoroutine(FoodEmptyNoNextDeliveryChlngMode());
                }
                else
                {
                    StartCoroutine(DelayForShowingNextChlngNotify());
                }
                    
            }
            print("nextbug222");
        }
       
        


        
        //minimap hiding if truck is empty
        if (GameManager.Instance.UIControllerInst.PlayerFoodSlider.value <= 0)
        {
            GameManager.Instance.PoorCharactersInstList[1].GetComponent<PoorGuyScript>().MinimapHotSpot.SetActive(false);
        }

        if(GameManager.Instance.ModeSelected == 0 && GameManager.Instance.UIControllerInst.PlayerFoodSlider.value <= 0)
        { }
        else
        GameManager.Instance.UIControllerInst.TimerAnimator.SetTrigger("newMission");
        //GameManager.Instance.UIControllerInst.NextDeliveryNotify.SetActive(true);
        //delivery time calculation
        //Vector3 direction = Vector3.zero;
        //if (vehicleWthWhlCldrs)
        //    direction = CurrentDeliveryPt.position - GameManager.Instance.CurrentVehicle.transform.position;//for wheeldrive
        //else
        //    direction = CurrentDeliveryPt.position - GameManager.Instance.PlayerVehicle.transform.position;//for kartController
        //Distance = Mathf.RoundToInt(direction.magnitude / 5);

        print("DistCheck_" + Distance);
        if(GameManager.Instance.ModeSelected==1)
        GameManager.Instance.TimeToDeliverSeconds = (float)timeToDeliver;
        print("TimeDeliver_" + GameManager.Instance.TimeToDeliverSeconds);

        //GameManager.Instance.deliveryPtObjcts.RemoveAt(0);
        print("NewDelPt_" + CurrentDeliveryPt.transform.position);
    }

    void selectCollectionPt()
    {
        GameManager.Instance.deliveryPtObjcts = ShuffleList(GameManager.Instance.deliveryPtObjcts);

        bool processBool = true;
        //current Collection point selection
        while (processBool)
        {
            if (CurrentCollectingPt != null)
                GameManager.Instance.deliveryPtObjcts.Add(CurrentCollectingPt);
            CurrentCollectingPt = GameManager.Instance.deliveryPtObjcts[0];
            GameManager.Instance.deliveryPtObjcts.RemoveAt(0);
            Vector3 direction = Vector3.zero;
   
            direction = CurrentCollectingPt.position - GameManager.Instance.CurrentVehicle.transform.position;//for wheeldrive
           
            int Distance = Mathf.RoundToInt(direction.magnitude / 5);

            if (GameManager.Instance.DeliveryCount < 2)
            {
                if (Distance < 50)
                {
                    processBool = false;
                }

            }
            else if (GameManager.Instance.DeliveryCount < 4)
            {
                if (Distance < 100 && Distance > 50)
                {
                    processBool = false;
                }
            }
            else
            {
                processBool = false;
            }
        }

        //donating guy generation
        if (GameManager.Instance.DonatingCharacterList.Count > 1)
        {
            print("donationCount_" + GameManager.Instance.DonatingCharacterList.Count);
            Destroy(GameManager.Instance.DonatingCharacterList[0]);
            GameManager.Instance.DonatingCharacterList.RemoveAt(0);
            print("donationCount_2_" + GameManager.Instance.DonatingCharacterList.Count);
        }
        GameObject donateInst = Instantiate(DonatingCharacter[Random.Range(0, DonatingCharacter.Length)], CurrentCollectingPt.position, CurrentCollectingPt.transform.rotation);//Random.Range(0,360)Quaternion.Euler(0,0,0)
        GameManager.Instance.DonatingCharacterList.Add(donateInst);
        //GameManager.Instance.NextPoorGuy = donateInst.GetComponent<PoorGuyScript>();


        ////minimap hiding if truck is empty
        //if (GameManager.Instance.UIControllerInst.PlayerFoodSlider.value <= 0)
        //{
        //    GameManager.Instance.PoorCharactersInstList[1].GetComponent<PoorGuyScript>().MinimapHotSpot.SetActive(false);
        //}


        //GameManager.Instance.UIControllerInst.TimerAnimator.SetTrigger("newMission");
    }


    IEnumerator DelayForShowingNextChlngNotify()//next challenge
    {
        
        
        if (GameManager.Instance.DeliveryCount != 0)
        {
            yield return new WaitForSeconds(4);
            GameManager.Instance.UIControllerInst.GiftBoxPopupFn();
        }
        else
        {
            yield return new WaitForSeconds(2);
        }
            
        print(GameManager.Instance.UIControllerInst.GiftBoxSlider.fillAmount + "fillamnt");
        if (GameManager.Instance.UIControllerInst.GiftBoxSlider.fillAmount >= 1)
        {
            
            //GameManager.Instance.UIControllerInst.GiftBoxSlider.fillAmount = 0;
            yield return new WaitForSeconds(5);
            GameManager.Instance.UIControllerInst.XPCalculationFn();
            if(GameManager.Instance.UIControllerInst.XPLvlUp_GmPlay.activeSelf)
                yield return new WaitForSeconds(3);
            GameManager.Instance.UIControllerInst.TutorialClicks(2);
            GameManager.Instance.UIControllerInst.GiftBoxSlider.fillAmount = 0;
        }
        else
        {
            GameManager.Instance.UIControllerInst.XPCalculationFn();
            if (GameManager.Instance.UIControllerInst.XPLvlUp_GmPlay.activeSelf)
                yield return new WaitForSeconds(3);
            GameManager.Instance.UIControllerInst.TutorialClicks(2);
        }
        
    }
    IEnumerator FoodEmptyNoNextDeliveryChlngMode()//No next challenge
    {


        if (GameManager.Instance.DeliveryCount != 0)
        {
            yield return new WaitForSeconds(4);
            GameManager.Instance.UIControllerInst.GiftBoxPopupFn();
        }
        else
        {
            yield return new WaitForSeconds(2);
        }

        print(GameManager.Instance.UIControllerInst.GiftBoxSlider.fillAmount + "fillamnt");
        if (GameManager.Instance.UIControllerInst.GiftBoxSlider.fillAmount >= 1)
        {

            //GameManager.Instance.UIControllerInst.GiftBoxSlider.fillAmount = 0;
            yield return new WaitForSeconds(5);
            GameManager.Instance.UIControllerInst.XPCalculationFn();
            if (GameManager.Instance.UIControllerInst.XPLvlUp_GmPlay.activeSelf)
                yield return new WaitForSeconds(3);
            //GameManager.Instance.UIControllerInst.TutorialClicks(2);
            GameManager.Instance.DriveStart = true;
            GameManager.Instance.UIControllerInst.controlsUIHideShow(1);
            GameManager.Instance.UIControllerInst.GiftBoxSlider.fillAmount = 0;
            print("nextbug2");
        }
        else
        {
            print("nextbug3");
            GameManager.Instance.UIControllerInst.XPCalculationFn();
            if (GameManager.Instance.UIControllerInst.XPLvlUp_GmPlay.activeSelf)
                yield return new WaitForSeconds(3);
            //GameManager.Instance.UIControllerInst.TutorialClicks(2);
            GameManager.Instance.DriveStart = true;
            GameManager.Instance.UIControllerInst.controlsUIHideShow(1);
            print("nextbug4");
        }


        GameManager.Instance.UIControllerInst.PlayerFoodSlider.transform.parent.gameObject.SetActive(true); GameManager.Instance.UIControllerInst.PlayerHealthSlider.transform.parent.gameObject.SetActive(true);
        GameManager.Instance.UIControllerInst.TimerAnimator.transform.parent.gameObject.SetActive(true); GameManager.Instance.UIControllerInst.MapHighlight.transform.parent.gameObject.SetActive(true);
        GameManager.Instance.UIControllerInst.CoinText.transform.parent.gameObject.SetActive(true); GameManager.Instance.UIControllerInst.DistanceText.gameObject.SetActive(true); GameManager.Instance.UIControllerInst.PauseBtn.SetActive(true);
        GameManager.Instance.UIControllerInst.GiftBoxSlider.transform.parent.gameObject.SetActive(true); GameManager.Instance.UIControllerInst.XPSliderGmplay.gameObject.SetActive(true);
        print("nextbug5");
    }
    public void StartRefillAnim()
    {
        print("startRefillANim");
        GameManager.Instance.CurrntVehicleContrlr.GiftBoxEfx.SetActive(false);//giftbox in vehicle efx
        StoreHotSpotList[GameManager.Instance.FoodStoreSelected].SetActive(false);StoreAnimatorList[GameManager.Instance.FoodStoreSelected].SetTrigger("refillAnim");
        if(GameManager.Instance.UIControllerInst)
        {
            GameManager.Instance.UIControllerInst.CoinAddEfx.SetActive(false);//giftbox coinADD efx
            GameManager.Instance.UIControllerInst.GiftBoxSliderEfx.SetActive(false);//giftbox slider efx
            GameManager.Instance.UIControllerInst.InGameUI.SetActive(false);
            GameManager.Instance.UIControllerInst.PlayerFoodSlider.value = GameManager.Instance.UIControllerInst.PlayerFoodSlider.maxValue;
            GameManager.Instance.UIControllerInst.FoodSliderAnimtr.enabled = false;
            GameManager.Instance.UIControllerInst.FoodSliderAnimtr.transform.localScale = new Vector3(1, 1, 1);
            GameManager.Instance.UIControllerInst.FoodEmptyNotfcn.SetActive(false);
        }
        if (vehicleWthWhlCldrs)
            GameManager.Instance.CurrentVehicle.gameObject.SetActive(false);//for wheeldrive

        //make store visible
        Renderer[] renderList = StoreRender.GetComponentsInChildren<Renderer>();
        foreach (var ren in renderList)
            ren.enabled = true;

        CamFoodStoreList[GameManager.Instance.FoodStoreSelected].SetActive(true);

        //challenge mode check
        if (GameManager.Instance.ModeSelected == 0 && GameManager.Instance.DeliveryCount!=0)
        {
            GameManager.Instance.CurrentRefillCount++;
            if (GameManager.Instance.SelectedChallenge.RefillChallenge)
            {
                //challenege completeness check
                GameManager.Instance.ChallengeCompletenessCheck();
                if (GameManager.Instance.ModeSelected == 0 && GameManager.Instance.challengeCompleted)
                {
                    GameManager.Instance.UIControllerInst.CallChallengeEnd(0);
                }
            }
        }
    }
    public void StartPlay()
    {
        print("startPlay");
        if(PlayerPrefs.GetInt("FirstStart")==0)
        {
            if(GameManager.Instance.ModeSelected==0)
            {
                if (GameManager.Instance.SelectedChallenge.TimeChallenge)
                    GameManager.Instance.TimeToDeliverSeconds = GameManager.Instance.SelectedChallenge.TimeCount;
            }
            PlayerPrefs.SetInt("FirstStart", 1);
            CollectablesAssign();
        }
        else
        {
            //if (GameManager.Instance.ModeSelected == 0)
            //{
            //    GameManager.Instance.CurrentRefillCount++;
            //}
            //GameManager.Instance.UIControllerInst.StoreMiniMapSpot.SetActive(false);
            GameManager.Instance.UIControllerInst.StoreMiniMapSpotList[GameManager.Instance.FoodStoreSelected].SetActive(false);
            GameManager.Instance.PoorCharactersInstList[1].GetComponent<PoorGuyScript>().MinimapHotSpot.SetActive(true);
        }
        
        StoreHotSpotList[GameManager.Instance.FoodStoreSelected].SetActive(true);//StoreAnimator.enabled = false;
        if(CurrentDeliveryPt==null)
        {
            print("selectDelivery");
            selectDeliveryPt();
        }
        if (CurrentCollectingPt == null)
        {
            selectCollectionPt();
        }

        GameManager.Instance.UIControllerInst.CountDwnTimer.SetActive(false);
        GameManager.Instance.UIControllerInst.InGameUI.SetActive(true);
        print(GameManager.Instance.DeliveryCount + "delcount");
        if(GameManager.Instance.DeliveryCount>0)
        {
            GameManager.Instance.UIControllerInst.GOtextObj.SetActive(false); GameManager.Instance.UIControllerInst.GOtextObj.SetActive(true);
        }
            
        if (vehicleWthWhlCldrs)
            GameManager.Instance.CurrentVehicle.gameObject.SetActive(true);//for wheeldrive
        else
        {
            if (GameManager.Instance.PlayerVehicle.parent != null)
                GameManager.Instance.PlayerVehicle.parent.gameObject.SetActive(true);//for kartController
            else
                GameManager.Instance.PlayerVehicle.gameObject.SetActive(true);
        }
           
        //GameplayCam.SetActive(true);
        //camFoodStore.SetActive(false);
        CamFoodStoreList[GameManager.Instance.FoodStoreSelected].SetActive(false);

        //challenge mode check
        //if (GameManager.Instance.ModeSelected == 0)
        //{
        //    //GameManager.Instance.CurrentRefillCount++;
        //    if(GameManager.Instance.SelectedChallenge.RefillChallenge)
        //    {
        //        //challenege completeness check
        //        GameManager.Instance.ChallengeCompletenessCheck();
        //        if (GameManager.Instance.ModeSelected == 0 && GameManager.Instance.challengeCompleted)
        //        {
        //            GameManager.Instance.UIControllerInst.CallChallengeEnd(0);
        //        }
        //    }
        //}

    }
    IEnumerator DelayForDeliveryTutorial()
    {
        yield return new WaitForSeconds(2);
        GameManager.Instance.UIControllerInst.DeliveryTut.SetActive(true);
        yield return new WaitForSeconds(2);
        Time.timeScale = 0;
        GameManager.Instance.CurrntVehicleContrlr.VehicleSoundOff();

    }
    public void DeliverFn()
    {
        GameManager.Instance.CurrntVehicleContrlr.IdleSound.mute = true;
        //GameManager.Instance.UIControllerInst.DeliverBtn.SetActive(false);
        GameManager.Instance.UIControllerInst.CoinAddEfx.SetActive(false);//giftbox coinADD efx
        GameManager.Instance.DriveStart = false;
        GameManager.Instance.DeliveryCount++;
        PlayerPrefs.SetInt("TotalDeliveryCount", PlayerPrefs.GetInt("TotalDeliveryCount") + 1);
        GameManager.Instance.UIControllerInst.DeliveryCountText.text = GameManager.Instance.DeliveryCount + "";
        GameManager.Instance.XPCalcDeliverCount++;
        GameManager.Instance.CurrentPoorGuy.FoodReceive();
        //GameManager.Instance.UIControllerInst.AudioList[0].Play();
        GameManager.Instance.UIControllerInst.PlayerFoodSlider.value--;
        if (GameManager.Instance.UIControllerInst.PlayerFoodSlider.value <= 1)
        {
            GameManager.Instance.UIControllerInst.FoodSliderAnimtr.enabled = true;
        }
            if (GameManager.Instance.UIControllerInst.PlayerFoodSlider.value<=0)
        {
            GameManager.Instance.UIControllerInst.FoodEmptyNotfcn.SetActive(true);
            //GameManager.Instance.UIControllerInst.StoreMiniMapSpotList[GameManager.Instance.FoodStoreSelected].SetActive(true);
            NearestStoreIndex = GetNearestFoodStore();
            GameManager.Instance.UIControllerInst.StoreMiniMapSpotList[NearestStoreIndex].SetActive(true);
        }

        if (PlayerPrefs.GetInt("TutDelivery") == 0)
        {
            print("tutdel1");
            PlayerPrefs.SetInt("TutDelivery", 1);
            StartCoroutine(DelayForDeliveryTutorial());
            GameManager.Instance.UIControllerInst.RoadArrowSigns.SetActive(false);
        }
        GameManager.Instance.TimeLeftAfterDelivery = GameManager.Instance.TimeToDeliverSeconds;
            selectDeliveryPt();
        switch(GameManager.Instance.DeliveryCount)
        {
            case 1:
                TrafficGenerate(1);
                break;
            case 2:
                TrafficGenerate(1);
                break;
            case 3:
                TrafficGenerate(1);
                break;
            case 4:
                TrafficGenerate(1);
                break;
        }
    }

    public void DeliverFnChallengeMode()
    {
        print("challengeModeDel1");
        GameManager.Instance.CurrntVehicleContrlr.IdleSound.mute = true;
        //GameManager.Instance.UIControllerInst.DeliverBtn.SetActive(false);
        GameManager.Instance.UIControllerInst.CoinAddEfx.SetActive(false);//giftbox coinADD efx
        GameManager.Instance.DriveStart = false;
        GameManager.Instance.DeliveryCount++;
        GameManager.Instance.XPCalcDeliverCount++;
        GameManager.Instance.CurrentPoorGuy.FoodReceive();
        //GameManager.Instance.UIControllerInst.AudioList[0].Play();
        GameManager.Instance.UIControllerInst.PlayerFoodSlider.value--;
        if (GameManager.Instance.UIControllerInst.PlayerFoodSlider.value <= 1)
        {
            GameManager.Instance.UIControllerInst.FoodSliderAnimtr.enabled = true;
        }
        if (GameManager.Instance.UIControllerInst.PlayerFoodSlider.value <= 0)
        {
            GameManager.Instance.UIControllerInst.FoodEmptyNotfcn.SetActive(true);
            //GameManager.Instance.UIControllerInst.StoreMiniMapSpotList[GameManager.Instance.FoodStoreSelected].SetActive(true);
            NearestStoreIndex = GetNearestFoodStore();
            GameManager.Instance.UIControllerInst.StoreMiniMapSpotList[NearestStoreIndex].SetActive(true);
        }

        //if (PlayerPrefs.GetInt("TutDelivery") == 0)
        //{
        //    PlayerPrefs.SetInt("TutDelivery", 1);
        //    StartCoroutine(DelayForDeliveryTutorial());
        //    GameManager.Instance.UIControllerInst.RoadArrowSigns.SetActive(false);
        //}
        GameManager.Instance.TimeLeftAfterDelivery = GameManager.Instance.TimeToDeliverSeconds;

        //challenge completeteness check

        if(GameManager.Instance.SelectedChallenge.DeliveryChallenge)
        {
            GameManager.Instance.ChallengeCompletenessCheck();
            //if (GameManager.Instance.DeliveryCount == GameManager.Instance.SelectedChallenge.DeliveryCount)
            //{
            //    //challenge completed
            //    GameManager.Instance.challengeCompleted = true;
            //}
            if (GameManager.Instance.challengeCompleted == true)
            {
                //challenge completed
                
            }
            else
            {
                //if (GameManager.Instance.UIControllerInst.PlayerFoodSlider.value > 0)
                {
                    selectDeliveryPt();
                }
                //else
                //{
                //    StartCoroutine(FoodEmptyNoNextDeliveryChlngMode());
                //}
                int TrafficCount = 1;
                if (PlayerPrefs.GetInt("ChallengeNum") >= 4)
                    TrafficCount = 2;
                switch (GameManager.Instance.DeliveryCount)
                {
                    case 1:
                        TrafficGenerate(TrafficCount);
                        break;
                    case 2:
                        TrafficGenerate(1);
                        break;
                    case 3:
                        TrafficGenerate(TrafficCount);
                        break;
                    case 4:
                        TrafficGenerate(1);
                        break;
                }
            }
            }

        //switch(PlayerPrefs.GetInt("ChallengeNum"))
        //{
        //    case 0://first challenge
        //        if(PlayerPrefs.GetInt("Environment")==0)//city  1 snow 2 desert
        //        {
        //            if(GameManager.Instance.DeliveryCount==1)
        //            {
        //                //challenge completed
        //                GameManager.Instance.challengeCompleted = true;
        //                                    }
        //        }
        //        break;
        //}

        //selectDeliveryPt();
        //switch (GameManager.Instance.DeliveryCount)
        //{
        //    case 1:
        //        TrafficGenerate(1);
        //        break;
        //    case 2:
        //        TrafficGenerate(1);
        //        break;
        //    case 3:
        //        TrafficGenerate(1);
        //        break;
        //    case 4:
        //        TrafficGenerate(1);
        //        break;
        //}
    }

    public void CollectingFn()
    {
        //GameManager.Instance.DriveStart = false;
        GameManager.Instance.CurrentDonateGuy.CollectedFood();
        //GameManager.Instance.UIControllerInst.AudioList[0].Play();
        GameManager.Instance.UIControllerInst.PlayerFoodSlider.value++;

        selectCollectionPt();
    }

    void TrafficGenerate(int count)
    {
        for(int i=0;i<GameManager.Instance.VehicleSpawnrList.Count;i++)
        {
            print(GameManager.Instance.VehicleSpawnrList[i] + "listcnt");
           StartCoroutine(GameManager.Instance.VehicleSpawnrList[i].SpawnTrafficDynamic(count));
        }
    }

    void CollectablesAssign()
    {
        for (int i=0;i<GameManager.Instance.collectablesPtObjcts.Count;i++)
        {
            if(Random.Range(0,5)>2 && (GameManager.Instance.ModeSelected==1))//careermode
            {
                if(Random.Range(0,2)>0)
                {
                    //timer repair kit
                    if (Random.Range(0, 2) > 0)
                    {
                        //timer
                        Instantiate(TimerPrefab,GameManager.Instance.collectablesPtObjcts[i].position,Quaternion.identity);
                    }
                    else
                    {
                        //repair
                        Instantiate(RepairPrefab, GameManager.Instance.collectablesPtObjcts[i].position, Quaternion.identity);
                    }
                }
                else
                {
                    //nothing will be spawned
                }
            }
            else
            {
                if (Random.Range(0, 3) > 0)
                {
                    //coins
                    Instantiate(CoinPrefab, GameManager.Instance.collectablesPtObjcts[i].position, Quaternion.identity);
                }
                else
                {
                    //nothing will be spawned
                }
            }
        }
    }
    List<Transform> ShuffleList(List<Transform> ListToShuffle)
    {
        for (int i = 0; i < ListToShuffle.Count; i++)
        {
            Transform temp = ListToShuffle[i];
            int randomIndex = Random.Range(i, ListToShuffle.Count);
            ListToShuffle[i] = ListToShuffle[randomIndex];
            ListToShuffle[randomIndex] = temp;
        }
        return ListToShuffle;
    }

}
