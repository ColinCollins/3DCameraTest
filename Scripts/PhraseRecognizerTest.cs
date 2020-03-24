//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UI;

//public class PhraseRecognizerTest : MonoBehaviour
//{
//    public InputField TextInput;
//    public Text Result;

//    KeywordRecognizer keywordRecognizer;
    
//    Dictionary<string, System.Action> keywords;

//    private string resultHandle = "";

//    // Start is called before the first frame update
//    void Start()
//    {
//        keywords = new Dictionary<string, System.Action>();

//        TextInput.onValueChanged.AddListener((value) => {
//            keywords.Add(value, showToResult);
//        });

//        keywords.Add("黄四郎", showToResult);

//        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

//        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
//    }

//    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
//    {
//        System.Action keywordAction;
//        // if the keyword recognized is in our dictionary, call that Action.
//        if (keywords.TryGetValue(args.text, out keywordAction))
//        {
//            resultHandle = args.text;
//            keywordAction.Invoke();
//        }
//    }

//    private void showToResult() {
//        Result.text = resultHandle;
//    }

//    private void Update()
//    {
//        if (Input.touchCount > 0) {
//            keywordRecognizer.Start();
//        }
//    }

//}
