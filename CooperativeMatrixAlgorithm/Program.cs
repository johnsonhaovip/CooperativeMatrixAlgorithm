using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMatrixAlgorithm
{
    /// <summary>
    /// 定義等級評定Model
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// 值
        /// </summary>
        public float Value
        {
            get;
            set;
        }
        /// <summary>
        ///频率/次数
        /// </summary>
        public int Freq
        {
            get;
            set;
        }
        /// <summary>
        /// 平均值
        /// </summary>
        public float AverageValue
        {
            get { return Value / Freq; }
        }
    }

    /// <summary>
    /// 等级差异收集
    /// </summary>
    public class RatingDifferenceCollection : Dictionary<string, Rating>
    {
        private string GetKey(int Item1Id, int Item2Id)
        {
            string Res = (Item1Id < Item2Id) ? Item1Id + "/" + Item2Id : Item2Id + "/" + Item1Id;
            return Res;
        }
        public bool Contains(int Item1Id, int Item2Id)
        {
            bool BRes = this.Keys.Contains<string>(GetKey(Item1Id, Item2Id));
            return BRes;
        }
        public Rating this[int Item1Id, int Item2Id]
        {
            get
            {
                return this[this.GetKey(Item1Id, Item2Id)];
            }
            set { this[this.GetKey(Item1Id, Item2Id)] = value; }
        }
    }
    /// <summary>
    /// 协同矩阵
    /// </summary>
    /// 创建者：Johnson
    /// 创建时间：2016-12-05
    public class CooperativeMatrixAlgorithm
    {
        public RatingDifferenceCollection _DiffMarix = new RatingDifferenceCollection();  // 協同矩陣字典 
        public HashSet<int> _Items = new HashSet<int>();  // 查看有多少個

        /// <summary>
        /// 接收用户打分记录
        /// </summary>
        /// <param name="userRatings"></param>
        public void AddUserRatings(IDictionary<int, float> userRatings)
        {
            foreach (var item1 in userRatings)
            {
                int item1Id = item1.Key;
                float item1Rating = item1.Value;
                _Items.Add(item1.Key);
                foreach (var item2 in userRatings)
                {
                    if (item2.Key <= item1Id)
                        continue; //消除多餘數據
                    int item2Id = item2.Key;
                    float item2Rating = item2.Value;
                    Rating ratingDiff;
                    if (_DiffMarix.Contains(item1Id, item2Id))
                    {
                        ratingDiff = _DiffMarix[item1Id, item2Id];
                    }
                    else
                    {
                        ratingDiff = new Rating();
                        _DiffMarix[item1Id, item2Id] = ratingDiff;
                    }
                    ratingDiff.Value += item1Rating - item2Rating;
                    ratingDiff.Freq += 1;//记录操作过的数
                }
            }
        }
        // 输入所有用户的评级
        public void AddUerRatings(IList<IDictionary<int, float>> Ratings)
        {
            foreach (var userRatings in Ratings)
            {
                AddUserRatings(userRatings);
            }
        }
        /// <summary>
        /// 预言/相似计算
        /// </summary>
        /// <param name="userRatings">用户输入参数</param>
        /// <returns>推算出对其它Items的可能评估的值</returns>
        /// 创建者：Johnson
        /// 创建时间：2016-12-05
        public IDictionary<int, float> Predict(IDictionary<int, float> userRatings)
        {
            Dictionary<int, float> Predictions = new Dictionary<int, float>();
            foreach (var itemId in this._Items)
            {
                if (userRatings.Keys.Contains(itemId))
                    continue; //如果用戶評價这一项,就跳过它 

                //用此用户的所有评估结合第一步得到的矩阵, 推算此用户对系统中每个项目的评估
                Rating itemRating = new Rating();
                foreach (var userRating in userRatings)
                {
                    if (userRating.Key == itemId)
                        continue;
                    int inputItemId = userRating.Key;
                    if (_DiffMarix.Contains(itemId, inputItemId))
                    {
                        Rating diff = _DiffMarix[itemId, inputItemId];
                        itemRating.Value += diff.Freq * (userRating.Value + diff.AverageValue * ((itemId < inputItemId) ? 1 : -1));
                        itemRating.Freq += diff.Freq;
                    }
                }
                Predictions.Add(itemId, itemRating.AverageValue);
            }
            return Predictions;
        }
        /// <summary>
        /// 测试协同矩阵
        /// </summary>
        /// <param name="args"></param>
        /// 创建者：Johnson
        /// 创建时间：2016-12-05
        static void Main(string[] args)
        {
            CooperativeMatrixAlgorithm test = new CooperativeMatrixAlgorithm();

            #region 其他用户对商品的评分数据，可以从大数据中获取。TODO现在为写死数据
            Dictionary<int, float> userRating = new Dictionary<int, float>();
            userRating.Add(1, 75);
            userRating.Add(2, 85);
            userRating.Add(3, 96);
            userRating.Add(4, 78);
            userRating.Add(5, 88);
            userRating.Add(6, 90);
            userRating.Add(7, 91);
            userRating.Add(8, 78);
            userRating.Add(9, 87);
            userRating.Add(10, 89);
            test.AddUserRatings(userRating);
            userRating = new Dictionary<int, float>();
            userRating.Add(1, 78);
            userRating.Add(2, 89);
            userRating.Add(3, 84);
            userRating.Add(4, 85);
            userRating.Add(5, 81);
            userRating.Add(6, 79);
            userRating.Add(7, 89);
            userRating.Add(8, 91);
            userRating.Add(9, 95);
            userRating.Add(10, 87);
            test.AddUserRatings(userRating);
            userRating = new Dictionary<int, float>();
            userRating.Add(1, 95);
            userRating.Add(2, 96);
            userRating.Add(3, 78);
            userRating.Add(4, 85);
            userRating.Add(5, 86);
            userRating.Add(6, 87);
            userRating.Add(7, 90);
            userRating.Add(8, 79);
            userRating.Add(9, 71);
            userRating.Add(10, 85);
            test.AddUserRatings(userRating);
            userRating = new Dictionary<int, float>();
            userRating.Add(1, 75);
            userRating.Add(2, 74);
            userRating.Add(3, 81);
            userRating.Add(4, 76);
            userRating.Add(5, 75);
            userRating.Add(6, 95);
            userRating.Add(7, 80);
            userRating.Add(8, 86);
            userRating.Add(9, 82);
            userRating.Add(10, 88);
            test.AddUserRatings(userRating);
            userRating = new Dictionary<int, float>();
            userRating.Add(1, 74);
            userRating.Add(2, 85);
            userRating.Add(3, 90);
            userRating.Add(4, 99);
            userRating.Add(5, 83);
            userRating.Add(6, 78);
            userRating.Add(7, 79);
            userRating.Add(8, 81);
            userRating.Add(9, 78);
            userRating.Add(10,80);
            test.AddUserRatings(userRating);
            #endregion

            try
            {
                //新测试数据
                Dictionary<int, float> testuserRating = new Dictionary<int, float>();
                testuserRating = new Dictionary<int, float>();

                int good1Value = 0, good2Value = 0;
                Console.WriteLine("请输入您对商品:1的评分，以判断您对其他商品的喜好。商品1的评分为（1-100）分");
                good1Value = Convert.ToInt32(Console.ReadLine());
                testuserRating.Add(1, good1Value);
                Console.WriteLine("请输入您对商品:2的评分，以更准确判断您对其他商品的喜好。商品2的评分为（1-100）分");
                good2Value = Convert.ToInt32(Console.ReadLine());
                testuserRating.Add(2, good2Value);

                //TODO:2017-11-22
                //这里采用SlopeOne算法
                if (testuserRating.Count > 0 && ((good1Value > 0 && good1Value <=100) && (good2Value <= 100 && good2Value > 0)))
                {
                    IDictionary<int, float> Predictions = test.Predict(testuserRating);
                    Console.WriteLine("您已经对商品1评分为:" + good1Value + "分，商品2评分为:" + good2Value + "分");
                    Console.WriteLine("通过《推荐系统》中的大数据以及您的喜好，得到您可能对其他商品的评分如下（注：其他商品评分在1-200）");
                    Console.WriteLine("評定結果:");
                    int loveGoodIndex = 0;//最应该购买的商品编号
                    float loveGoodValue = 0;//最应该购买的商品得分
                    foreach (var rating in Predictions)
                    {
                        if (rating.Value > loveGoodValue)
                        {
                            loveGoodValue = rating.Value;
                            loveGoodIndex = rating.Key;
                        }
                        Console.WriteLine("項次/項目(Item)/商品:" + rating.Key);
                        Console.WriteLine("等級評定值/分(Rating):" + rating.Value);
                        Console.ReadLine();
                    }
                    Console.WriteLine("因此接下来您最应该购买的商品编号为：" + loveGoodIndex + ";它的分值为：" + loveGoodValue);
                    Console.ReadLine();
                }
                else if (good1Value > 100 || good1Value < 0 || good2Value > 100 || good2Value < 0)
                {
                    Console.WriteLine("请按评分规则打分！谢谢合作");
                    Console.ReadLine();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Sorry！没有您喜好，无法预测您对其他商品的评分！");
                Console.ReadLine();
            }
        }
    }
}
