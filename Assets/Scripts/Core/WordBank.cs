using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the word pool for the typing system
/// </summary>
public class WordBank : MonoBehaviour
{
    private List<string> originalWords = new List<string>
    {
        "The quick brown fox jumps over the lazy dog.",
        "A journey of a thousand miles begins with a single step.",
        "To be or not to be, that is the question.",
        "All that glitters is not gold.",
        "I think, therefore I am.",
        "The only thing we have to fear is fear itself.",
        "Practice makes perfect.",
        "Actions speak louder than words.",
        "Time heals all wounds.",
        "The early bird catches the worm.",
        "Knowledge is power.",
        "Where there is a will, there is a way.",
        "Every cloud has a silver lining.",
        "Do not count your chickens before they hatch.",
        "The pen is mightier than the sword.",
        "Absence makes the heart grow fonder.",
        "In the middle of difficulty lies opportunity.",
        "It does not do to dwell on dreams and forget to live.",
        "The greatest glory in living lies not in never falling, but in rising every time we fall.",
        "Success is not final, failure is not fatal, it is the courage to continue that counts.",
        "The only way to do great work is to love what you do.",
        "Life is what happens when you are busy making other plans.",
        "The future belongs to those who believe in the beauty of their dreams.",
        "Strive not to be a success, but rather to be of value.",
        "What lies behind us and what lies before us are tiny matters compared to what lies within us.",
        "The purple giraffe danced under the moonlight.",
        "Flamingos are just pink clouds with legs.",
        "Why did the scarecrow win an award? Because he was outstanding in his field!",
        "The quick witted fox outsmarted the lazy hound.",
        "Jellyfish wobble like gelatin in the ocean breeze."
    };

    private List<string> easyWords = new List<string>
    {
        "Better late than never.",
        "Practice makes perfect.",
        "Time flies fast.",
        "Easy come, easy go.",
        "No pain, no gain.",
        "A deal is a deal.",
        "Actions speak louder.",
        "All good things end.",
        "Well begun is half done.",
        "Live and learn.",
        "Do not count on luck.",
        "Every little bit helps.",
        "Look before you leap.",
        "No news is good news.",
        "One step at a time.",
        "Silence is golden.",
        "Less is more.",
        "Hope for the best.",
        "Curiosity killed the cat."
    };

    // Runtime lists
    private List<string> workingWords = new List<string>();
    private List<string> workingEasyWords = new List<string>();

    private void Awake()
    {
        SetupList();
    }

    // Resets and shuffles both word lists
    private void SetupList()
    {
        workingWords.Clear();
        workingEasyWords.Clear();

        workingWords.AddRange(originalWords);
        workingEasyWords.AddRange(easyWords);

        Shuffle(workingWords);
        Shuffle(workingEasyWords);
    }

    private void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    // Returns a word from the respective list, refilling and reshuffling if empty
    public string GetWord(bool useEasyWords)
    {
        List<string> list = useEasyWords ? workingEasyWords : workingWords;

        if (list.Count == 0)
        {
            SetupList();
        }

        string newWord = list.Last();
        list.RemoveAt(list.Count - 1);
        return newWord;
    }
}