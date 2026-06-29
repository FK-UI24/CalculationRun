using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Script_CalculateManagement : MonoBehaviour
{
    // 難易度変えるならここの確率調整と制限時間短めでいいかも
    //四則演算生成の仕組みはAIがつくった。今は作りたい優先だから理解はしない。
    //これが新時代のゲーム制作ってね！（時間があるときにする）

    [Header("項数の確率")]
    [Range(0f, 1f)] public float weight2Terms = 0.4f; // 2項の式（例: A + B）
    [Range(0f, 1f)] public float weight3Terms = 0.3f; // 3項の式（例: A + B - C）
    [Range(0f, 1f)] public float weight4Terms = 0.2f; // 4項の式（例: A + B - C * D）
    [Range(0f, 1f)] public float weight5Terms = 0.1f; // 5項の式（例: A + B - C * D / E）

    [Header("演算子の確率")]
    [Range(0f, 1f)] public float weightPlus = 0.25f;       // 足し算（+）の出現確率
    [Range(0f, 1f)] public float weightMinus = 0.25f;      // 引き算（-）の出現確率
    [Range(0f, 1f)] public float weightMultiply = 0.25f;   // 掛け算（*）の出現確率
    [Range(0f, 1f)] public float weightDivide = 0.25f;     // 割り算（/）の出現確率

    [Header("計算時間の制限時間")]
    [SerializeField] private int countdown;

    [Header("制限時間テキスト")]
    [SerializeField] private TMP_Text countdownText;

    [Header("四則演算を表示するテキスト")]
    [SerializeField] private TMP_Text calculateText;

    [Header("復活させる足場")]
    [SerializeField] private Transform tileParent;

    //答えを格納する用変数
    private int ans;

    //タイルを消す直前のタイルを格納する用変数
    private List<GameObject> objs= new List<GameObject>();

    void Start()
    {
        //式を生成し、テキストに表示と答えを保存
        GenerateExpression();

        //カウントダウンテキストに初期値を入れる
        countdownText.text = countdown.ToString();

    }

    //OnEnableは有効になるたびに実行する
    //今回これがアタッチされているオブジェクトは最初に別スクリプトから無効化されるのでうまいこと動かない
    //なので有効になるたびに動かすようにする
    private void OnEnable()
    {
        StartCoroutine(countDownCalculate());
    }

    //カウントダウンを行い、式を表示
    private IEnumerator countDownCalculate()
    {
        while (true)
        {
            int timer = countdown;

            if (objs != null && ans != 0)
            {
                for (int i = 0; i < objs.Count; i++)
                {
                    objs[i].SetActive(true);
                }
            }

            objs.Clear();

            //タイマーがー0より大きい間
            while (timer >= 0)
            {
                //タイマーの現在の時間をカウントダウンテキストに入れる
                countdownText.text = timer.ToString();

                //1秒待つ
                yield return new WaitForSeconds(1f);

                //１秒待ったのでタイマーから１引く
                timer--;
            }

            //ここでタイルを消す
            for(int i = 1; i <= 9; i++)
            {
                if (i == ans) continue;
                objs.AddRange(GameObject.FindGameObjectsWithTag("Field_" + i));
            }

            for(int i = 0; i < objs.Count; i++)
            {
                objs[i].SetActive(false);
            }


            yield return new WaitForSeconds(1f);

            GenerateExpression();
        }
    }

    // 条件に合致した数式を1つ組み立てて出力する
    private void GenerateExpression()
    {
        // 最大10回まで安全にリトライ（制約が厳しいため極稀に詰むのを防ぐ王道処理）
        for (int attempt = 0; attempt < 10; attempt++)
        {
            int targetTermsCount = ChooseTermsCount(); // 今回の式に使う項の数（2〜5）を決定

            List<int> numbers = new List<int>();       // 数式に登場する数字（1〜9）を格納するリスト
            List<char> operators = new List<char>();   // 数式に登場する記号（+, -, *, /）を格納するリスト

            // 第1項（最初の数字）の決定 (1〜9)
            numbers.Add(Random.Range(1, 10));

            bool success = true; // 数式の組み立てが条件を壊さずに成功したかを管理するフラグ

            for (int i = 1; i < targetTermsCount; i++)
            {
                char op = ChooseOperator(); // 確率に基づいて演算子記号を選択
                int nextValue = FindValidNextValueNormalOrder(numbers, operators, op); // 条件をクリアできる安全な次の数字を探索

                if (nextValue == -1)
                {
                    success = false;
                    break; // この構成では作れないのでリトライ
                }

                operators.Add(op);
                numbers.Add(nextValue);
            }

            if (success)
            {
                // 表示用の数式文字列を構築
                string expression = numbers[0].ToString();
                for (int i = 0; i < operators.Count; i++)
                {
                    char displayOp = operators[i] switch
                    {
                        '*' => '×',
                        '/' => '÷',
                        _ => operators[i]
                    };
                    expression += $" {displayOp} {numbers[i + 1]}";
                }

                // 通常の四則演算順序（乗除先取）に従って最終的な答えを算出
                int finalResult = EvaluateNormalOrder(numbers, operators);

                //式をテキストに入れる
                calculateText.text = expression.ToString();

                //答えを格納する
                ans = finalResult;

                Debug.Log($"[通常順序] 生成された式: {expression} = {finalResult}");
                return;
            }
        }

        Debug.LogWarning("条件に合う式を時間内に生成できませんでした。確率のバランスを調整してください。");
    }

    // 通常の四則演算順序に基づいて、次の安全な値を探索する
    private int FindValidNextValueNormalOrder(List<int> currentNumbers, List<char> currentOperators, char nextOp)
    {
        List<int> validNumbers = new List<int>(); // ルールをクリアした数字だけを入れる合格リスト

        // 候補となる数字「1」から「9」までを1つずつシミュレーション
        for (int candidate = 1; candidate <= 9; candidate++)
        {
            // 本番のデータを汚さないようにシミュレーション用のコピーを作成
            List<int> testNumbers = new List<int>(currentNumbers);
            List<char> testOperators = new List<char>(currentOperators);

            testOperators.Add(nextOp);
            testNumbers.Add(candidate);

            // この段階での「すべての途中計算」が安全かチェック
            if (CheckIntermediateSteps(testNumbers, testOperators))
            {
                validNumbers.Add(candidate); // 安全に使用可能なら合格リストに追加
            }
        }

        if (validNumbers.Count == 0) return -1; // 合格者が0個の場合はエラーを示す -1 を返す
        return validNumbers[Random.Range(0, validNumbers.Count)]; // 合格リストからランダムに1つ選択
    }

    // 乗除先取のルールで計算し、かつ「すべての途中経過」が1〜9かつ割り切れているかを検証
    private bool CheckIntermediateSteps(List<int> nums, List<char> ops)
    {
        // 計算によって中身を減らしていくための作業用リストにコピー
        List<int> numbers = new List<int>(nums);
        List<char> operators = new List<char>(ops);

        // 1. まず掛け算・割り算を左から順に処理（乗除先取）
        for (int i = 0; i < operators.Count; i++)
        {
            if (operators[i] == '*' || operators[i] == '/')
            {
                int a = numbers[i];
                int b = numbers[i + 1];
                int res = 0;

                if (operators[i] == '*')
                {
                    res = a * b;
                }
                else if (operators[i] == '/')
                {
                    if (b == 0 || a % b != 0) return false; // 0での割り算、または割り切れない場合は即座に弾く
                    res = a / b;
                }

                if (res < 1 || res > 9) return false; // 途中結果が1〜9以外なら弾く

                // リストの更新（計算した2つの項を1つの結果に置換）
                numbers[i] = res;
                numbers.RemoveAt(i + 1);
                operators.RemoveAt(i);
                i--; // リストの要素数が減ったためインデックスを戻す
            }
        }

        // 2. 次に足し算・引き算を左から順に処理
        while (operators.Count > 0)
        {
            int a = numbers[0];
            int b = numbers[1];
            int res = 0;

            if (operators[0] == '+') res = a + b;
            else if (operators[0] == '-') res = a - b;

            if (res < 1 || res > 9) return false; // 途中結果（または最終結果）が1〜9以外（負の数や0も含む）なら弾く

            numbers[0] = res;
            numbers.RemoveAt(1);
            operators.RemoveAt(0);
        }

        return true; // 一度もルール違反に引っかからずに完走できたら安全
    }

    // 最終的な計算結果を返すヘルパー（CheckIntermediateStepsとほぼ同等）
    private int EvaluateNormalOrder(List<int> nums, List<char> ops)
    {
        List<int> numbers = new List<int>(nums);
        List<char> operators = new List<char>(ops);

        // 掛け算・割り算を先に処理
        for (int i = 0; i < operators.Count; i++)
        {
            if (operators[i] == '*' || operators[i] == '/')
            {
                int res = (operators[i] == '*') ? numbers[i] * numbers[i + 1] : numbers[i] / numbers[i + 1];
                numbers[i] = res;
                numbers.RemoveAt(i + 1);
                operators.RemoveAt(i);
                i--;
            }
        }

        // 残った足し算・引き算を処理
        while (operators.Count > 0)
        {
            int res = (operators[0] == '+') ? numbers[0] + numbers[1] : numbers[0] - numbers[1];
            numbers[0] = res;
            numbers.RemoveAt(1);
            operators.RemoveAt(0);
        }
        return numbers[0];
    }

    // 重みを元に、生成する式の項数（2〜5）を決定する
    private int ChooseTermsCount()
    {
        float total = weight2Terms + weight3Terms + weight4Terms + weight5Terms;
        float randomPoint = Random.Range(0f, total);

        if (randomPoint < weight2Terms) return 2;
        if (randomPoint < weight2Terms + weight3Terms) return 3;
        if (randomPoint < weight2Terms + weight3Terms + weight4Terms) return 4;
        return 5;
    }

    // 重みを元に、使用する演算子記号を1つ決定する
    private char ChooseOperator()
    {
        float total = weightPlus + weightMinus + weightMultiply + weightDivide;
        float randomPoint = Random.Range(0f, total);

        if (randomPoint < weightPlus) return '+';
        if (randomPoint < weightPlus + weightMinus) return '-';
        if (randomPoint < weightPlus + weightMinus + weightMultiply) return '*';
        return '/';
    }
}