using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialControl : MonoBehaviour
{
    [System.Serializable]
    public class Tutorial
    {
        public GameObject tutorialObject;
        public int checkPoint;
        public Animator complete;
        public GameObject correct;
        public Sprite bookPage1, bookPage2;
    }
    
    public List<Tutorial> tutorials;
    public int currentCheckPoint;
    private int _currentId;
    public bool checkEat = false;
    public bool checkWeight;
    public List<Manager> characters;
    public List<Sprite> initialPage;
    public Book book;
    private void Start()
    {
        TutorialManager.Controller = this;
        book.bookPages = new List<Sprite>(initialPage);
    }


    public void ShowTutorial(int id)
    {
        currentCheckPoint = 0;
        if (id > tutorials.Count)
        {
            Debug.LogError("教程id超出索引");
            return;
        }

        if (id == 2)
        {
            checkEat = true;
            StartCoroutine(CheckEat());
        }
        if (id == 3)
        {
            checkWeight = true;
            StartCoroutine(CheckWeight());
        }

        _currentId = id;
        
        TutorialManager.TutorialIsShow = true;
        tutorials[id].tutorialObject.SetActive(true);
    }

    public void AddCheckPoint()
    {
        currentCheckPoint++;
        if (currentCheckPoint >= tutorials[_currentId].checkPoint)
        {
            Invoke(nameof(HideTutorial),0.5f);
        }
    }
    
    public void HideTutorial()
    {
        tutorials[_currentId].tutorialObject.GetComponent<Animator>().SetTrigger("Hide");
        TutorialManager.TutorialIsShow = false;
        TutorialManager.TutorialWeight = false;
        currentCheckPoint = 0;
        book.bookPages.Add(tutorials[_currentId].bookPage1);
        book.bookPages.Add(tutorials[_currentId].bookPage2);
        Invoke(nameof(DestroyTutorial),0.5f);
    }
    
    
    public void DestroyTutorial()
    {
        tutorials[_currentId].tutorialObject.SetActive(false);
    }

    public void Shake()
    {
        tutorials[_currentId].tutorialObject.GetComponent<Animator>().SetTrigger("Shake");
    }


    

    private IEnumerator CheckEat()
    {
        while (checkEat)
        {
            if (TutorialManager.TutorialIsShow)
            {
                foreach (var item in characters)
                {
                    if (item.Eat)
                    {
                        tutorials[2].complete.SetTrigger("Comfirm");
                        tutorials[2].correct.SetActive(true);
                        checkEat = false;
                        AddCheckPoint();
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
            
        }
    }
    
    private IEnumerator CheckWeight()
    {
        while (checkWeight)
        {
            yield return new WaitForSeconds(0.5f);
            TutorialManager.TutorialWeight = true;
            foreach (var item in characters)
            {
                if (item.Weight > 0)
                {
                    tutorials[3].complete.SetTrigger("Comfirm");
                    checkWeight = false;
                    AddCheckPoint();
                    break;
                }
            }

            
            
        }
        
    }
    
    
}

public static class TutorialManager
{
    public static TutorialControl Controller;
    public static bool TutorialIsShow { get; set; }
    public static bool TutorialWeight { get; set;}
    public static bool CharacterIsTalking { get; set; }
}


