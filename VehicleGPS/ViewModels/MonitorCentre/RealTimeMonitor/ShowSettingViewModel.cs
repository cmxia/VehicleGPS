using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows;
using System.Xml;
using System.IO;
using System.Reflection;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models;

namespace VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor
{
    class ShowSettingViewModel : NotificationObject
    {
        private static ShowSettingViewModel instance = null;
        private ShowSettingViewModel()
        {
            this.BackToDefaultCommand = new DelegateCommand(new Action(this.BackToDefault));
            this.BackToDefault();
            //this.UserShowSetting();
        }
        public static ShowSettingViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new ShowSettingViewModel();
            }
            return instance;
        }
        /*还原默认*/
        public DelegateCommand BackToDefaultCommand { get; set; }
        private void BackToDefault()
        {
            string path = VehicleConfig.GetInstance().showSettingPath;
            if (!File.Exists(path))
            {
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode xn = xmlDoc.SelectSingleNode("Setting");
            // 得到根节点的所有子节点
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode node in xnl)
            {
                // 将节点转换为元素，便于得到节点的属性值
                XmlElement xe = (XmlElement)node;
                if (xe.GetAttribute("userName").ToString() == "Default")
                {
                    // 得到默认设置的所有子节点
                    XmlNodeList xnlDefault = xe.ChildNodes;
                    foreach (XmlNode nodeDefualt in xnlDefault)
                    {
                        string name = nodeDefualt.Name;
                        bool bshow = (nodeDefualt.InnerText == "0" ? false : true);
                        this.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance).SetValue(this, bshow, null);
                        //fieldInfo.SetValue(this,bshow);
                        //this.GetType().GetField(name).SetValue(this,bshow);
                    }
                    break;
                }
            }
        }
        /*用户下的配置*/
        private void UserShowSetting()
        {

        }
        #region 车辆基本信息
        public bool sequence = true;//序号
        public bool Sequence
        {
            get { return sequence; }
            set
            {
                if (sequence != value)
                {
                    sequence = value;
                    this.RaisePropertyChanged("Sequence");
                    if (sequence == true)
                    {
                        this.SequenceV = Visibility.Visible;
                    }
                    else
                    {
                        this.SequenceV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility sequenceV = Visibility.Visible;//序号
        public Visibility SequenceV
        {
            get { return sequenceV; }
            set
            {
                if (sequenceV != value)
                {
                    sequenceV = value;
                    this.RaisePropertyChanged("SequenceV");
                }
            }
        }

        private bool fInnerId = false;//内部编号
        public bool FInnerId
        {
            get { return fInnerId; }
            set
            {
                if (fInnerId != value)
                {
                    fInnerId = value;
                    this.RaisePropertyChanged("FInnerId");
                    if (fInnerId == true)
                    {
                        this.FInnerIdV = Visibility.Visible;
                    }
                    else
                    {
                        this.FInnerIdV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility fInnerIdV = Visibility.Collapsed;//内部编号
        public Visibility FInnerIdV
        {
            get { return fInnerIdV; }
            set
            {
                if (fInnerIdV != value)
                {
                    fInnerIdV = value;
                    this.RaisePropertyChanged("FInnerIdV");
                }
            }
        }

        private bool vehicleId = true;//车牌号码
        public bool VehicleId
        {
            get { return vehicleId; }
            set
            {
                if (vehicleId != value)
                {
                    vehicleId = value;
                    this.RaisePropertyChanged("VehicleId");
                    if (vehicleId == true)
                    {
                        this.VehicleIdV = Visibility.Visible;
                    }
                    else
                    {
                        this.VehicleIdV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vehicleIdV = Visibility.Visible;//车牌号码
        public Visibility VehicleIdV
        {
            get { return vehicleIdV; }
            set
            {
                if (vehicleIdV != value)
                {
                    vehicleIdV = value;
                    this.RaisePropertyChanged("VehicleIdV");
                }
            }
        }

        private bool sim = false;//SIM卡号
        public bool SIM
        {
            get { return sim; }
            set
            {
                if (sim != value)
                {
                    sim = value;
                    this.RaisePropertyChanged("SIM");
                    if (sim == true)
                    {
                        this.SIMV = Visibility.Visible;
                    }
                    else
                    {
                        this.SIMV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility simV = Visibility.Collapsed;//SIM卡号
        public Visibility SIMV
        {
            get { return simV; }
            set
            {
                if (simV != value)
                {
                    simV = value;
                    this.RaisePropertyChanged("SIMV");
                }
            }
        }

        private bool vehiclecurState = true;//车辆状态
        public bool VehiclecurState
        {
            get { return vehiclecurState; }
            set
            {
                if (vehiclecurState != value)
                {
                    vehiclecurState = value;
                    this.RaisePropertyChanged("VehiclecurState");
                    if (vehiclecurState == true)
                    {
                        this.VehiclecurStateV = Visibility.Visible;
                    }
                    else
                    {
                        this.VehiclecurStateV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vehiclecurStateV = Visibility.Visible;//车辆状态
        public Visibility VehiclecurStateV
        {
            get { return vehiclecurStateV; }
            set
            {
                if (vehiclecurStateV != value)
                {
                    vehiclecurStateV = value;
                    this.RaisePropertyChanged("VehiclecurStateV");
                }
            }
        }

        private bool customerName = true;//所属单位
        public bool CustomerName
        {
            get { return customerName; }
            set
            {
                if (customerName != value)
                {
                    customerName = value;
                    this.RaisePropertyChanged("CustomerName");
                    if (customerName == true)
                    {
                        this.CustomerNameV = Visibility.Visible;
                    }
                    else
                    {
                        this.CustomerNameV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility customerNameV = Visibility.Visible;//所属单位
        public Visibility CustomerNameV
        {
            get { return customerNameV; }
            set
            {
                if (customerNameV != value)
                {
                    customerNameV = value;
                    this.RaisePropertyChanged("CustomerNameV");
                }
            }
        }

        private bool vehicleTypeName = true;//车辆类别
        public bool VehicleTypeName
        {
            get { return vehicleTypeName; }
            set
            {
                if (vehicleTypeName != value)
                {
                    vehicleTypeName = value;
                    this.RaisePropertyChanged("VehicleTypeName");
                    if (vehicleTypeName == true)
                    {
                        this.VehicleTypeNameV = Visibility.Visible;
                    }
                    else
                    {
                        this.VehicleTypeNameV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vehicleTypeNameV = Visibility.Visible;//车辆类别
        public Visibility VehicleTypeNameV
        {
            get { return vehicleTypeNameV; }
            set
            {
                if (vehicleTypeNameV != value)
                {
                    vehicleTypeNameV = value;
                    this.RaisePropertyChanged("VehicleTypeNameV");
                }
            }
        }

        private bool brandModel = false;//厂牌型号
        public bool BrandModel
        {
            get { return brandModel; }
            set
            {
                if (brandModel != value)
                {
                    brandModel = value;
                    this.RaisePropertyChanged("BrandModel");
                    if (brandModel == true)
                    {
                        this.BrandModelV = Visibility.Visible;
                    }
                    else
                    {
                        this.BrandModelV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility brandModelV = Visibility.Collapsed;//厂牌型号
        public Visibility BrandModelV
        {
            get { return brandModelV; }
            set
            {
                if (brandModelV != value)
                {
                    brandModelV = value;
                    this.RaisePropertyChanged("BrandModelV");
                }
            }
        }

        private bool oilType = false;//燃油类型
        public bool OilType
        {
            get { return oilType; }
            set
            {
                if (oilType != value)
                {
                    oilType = value;
                    this.RaisePropertyChanged("OilType");
                    if (oilType == true)
                    {
                        this.OilTypeV = Visibility.Visible;
                    }
                    else
                    {
                        this.OilTypeV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility oilTypeV = Visibility.Collapsed;//燃油类型
        public Visibility OilTypeV
        {
            get { return oilTypeV; }
            set
            {
                if (oilTypeV != value)
                {
                    oilTypeV = value;
                    this.RaisePropertyChanged("OilTypeV");
                }
            }
        }

        private bool vin = false;//车架号
        public bool VIN
        {
            get { return vin; }
            set
            {
                if (vin != value)
                {
                    vin = value;
                    this.RaisePropertyChanged("VIN");
                    if (vin == true)
                    {
                        this.VINV = Visibility.Visible;
                    }
                    else
                    {
                        this.VINV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vinV = Visibility.Collapsed;//车架号
        public Visibility VINV
        {
            get { return vinV; }
            set
            {
                if (vinV != value)
                {
                    vinV = value;
                    this.RaisePropertyChanged("VINV");
                }
            }
        }

        private bool engineId = false;//发动机号
        public bool EngineId
        {
            get { return engineId; }
            set
            {
                if (engineId != value)
                {
                    engineId = value;
                    this.RaisePropertyChanged("EngineId");
                    if (engineId == true)
                    {
                        this.EngineIdV = Visibility.Visible;
                    }
                    else
                    {
                        this.EngineIdV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility engineIdV = Visibility.Collapsed;//发动机号
        public Visibility EngineIdV
        {
            get { return engineIdV; }
            set
            {
                if (engineIdV != value)
                {
                    engineIdV = value;
                    this.RaisePropertyChanged("EngineIdV");
                }
            }
        }

        private bool vehicleModel = false;//车辆型号
        public bool VehicleModel
        {
            get { return vehicleModel; }
            set
            {
                if (vehicleModel != value)
                {
                    vehicleModel = value;
                    this.RaisePropertyChanged("VehicleModel");
                    if (vehicleModel == true)
                    {
                        this.VehicleModelV = Visibility.Visible;
                    }
                    else
                    {
                        this.VehicleModelV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vehicleModelV = Visibility.Collapsed;//车辆型号
        public Visibility VehicleModelV
        {
            get { return vehicleModelV; }
            set
            {
                if (vehicleModelV != value)
                {
                    vehicleModelV = value;
                    this.RaisePropertyChanged("VehicleModelV");
                }
            }
        }

        private bool purchaseNum = false;//购置证号
        public bool PurchaseNum
        {
            get { return purchaseNum; }
            set
            {
                if (purchaseNum != value)
                {
                    purchaseNum = value;
                    this.RaisePropertyChanged("PurchaseNum");
                    if (purchaseNum == true)
                    {
                        this.PurchaseNumV = Visibility.Visible;
                    }
                    else
                    {
                        this.PurchaseNumV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility purchaseNumV = Visibility.Collapsed;//购置证号
        public Visibility PurchaseNumV
        {
            get { return purchaseNumV; }
            set
            {
                if (purchaseNumV != value)
                {
                    purchaseNumV = value;
                    this.RaisePropertyChanged("PurchaseNumV");
                }
            }
        }

        private bool operatNum = false;//营运证号
        public bool OperatNum
        {
            get { return operatNum; }
            set
            {
                if (operatNum != value)
                {
                    operatNum = value;
                    this.RaisePropertyChanged("OperatNum");
                    if (operatNum == true)
                    {
                        this.OperatNumV = Visibility.Visible;
                    }
                    else
                    {
                        this.OperatNumV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility operatNumV = Visibility.Collapsed;//营运证号
        public Visibility OperatNumV
        {
            get { return operatNumV; }
            set
            {
                if (operatNumV != value)
                {
                    operatNumV = value;
                    this.RaisePropertyChanged("OperatNumV");
                }
            }
        }

        private bool vehicleColor = false;//车辆颜色
        public bool VehicleColor
        {
            get { return vehicleColor; }
            set
            {
                if (vehicleColor != value)
                {
                    vehicleColor = value;
                    this.RaisePropertyChanged("VehicleColor");
                    if (vehicleColor == true)
                    {
                        this.VehicleColorV = Visibility.Visible;
                    }
                    else
                    {
                        this.VehicleColorV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vehicleColorV = Visibility.Collapsed;//车辆颜色
        public Visibility VehicleColorV
        {
            get { return vehicleColorV; }
            set
            {
                if (vehicleColorV != value)
                {
                    vehicleColorV = value;
                    this.RaisePropertyChanged("VehicleColorV");
                }
            }
        }

        private bool tonnage = false;//吨位
        public bool Tonnage
        {
            get { return tonnage; }
            set
            {
                if (tonnage != value)
                {
                    tonnage = value;
                    this.RaisePropertyChanged("Tonnage");
                    if (tonnage == true)
                    {
                        this.TonnageV = Visibility.Visible;
                    }
                    else
                    {
                        this.TonnageV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility tonnageV = Visibility.Collapsed;//吨位
        public Visibility TonnageV
        {
            get { return tonnageV; }
            set
            {
                if (tonnageV != value)
                {
                    tonnageV = value;
                    this.RaisePropertyChanged("TonnageV");
                }
            }
        }

        private bool tiresNum = false;//轮胎数
        public bool TiresNum
        {
            get { return tiresNum; }
            set
            {
                if (tiresNum != value)
                {
                    tiresNum = value;
                    this.RaisePropertyChanged("TiresNum");
                    if (tiresNum == true)
                    {
                        this.TiresNumV = Visibility.Visible;
                    }
                    else
                    {
                        this.TiresNumV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility tiresNumV = Visibility.Collapsed;//轮胎数
        public Visibility TiresNumV
        {
            get { return tiresNumV; }
            set
            {
                if (tiresNumV != value)
                {
                    tiresNumV = value;
                    this.RaisePropertyChanged("TiresNumV");
                }
            }
        }

        private bool emptyCost = false;//百公里油耗（空载）
        public bool EmptyCost
        {
            get { return emptyCost; }
            set
            {
                if (emptyCost != value)
                {
                    emptyCost = value;
                    this.RaisePropertyChanged("EmptyCost");
                    if (emptyCost == true)
                    {
                        this.EmptyCostV = Visibility.Visible;
                    }
                    else
                    {
                        this.EmptyCostV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility emptyCostV = Visibility.Collapsed;//百公里油耗（空载）
        public Visibility EmptyCostV
        {
            get { return emptyCostV; }
            set
            {
                if (emptyCostV != value)
                {
                    emptyCostV = value;
                    this.RaisePropertyChanged("EmptyCostV");
                }
            }
        }

        private bool fullCost = false;//百公里油耗（满载
        public bool FullCost
        {
            get { return fullCost; }
            set
            {
                if (fullCost != value)
                {
                    fullCost = value;
                    this.RaisePropertyChanged("FullCost");
                    if (fullCost == true)
                    {
                        this.FullCostV = Visibility.Visible;
                    }
                    else
                    {
                        this.FullCostV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility fullCostV = Visibility.Collapsed;//百公里油耗（满载
        public Visibility FullCostV
        {
            get { return fullCostV; }
            set
            {
                if (fullCostV != value)
                {
                    fullCostV = value;
                    this.RaisePropertyChanged("FullCostV");
                }
            }
        }

        private bool loadAmount = false;//核定承载方量
        public bool LoadAmount
        {
            get { return loadAmount; }
            set
            {
                if (loadAmount != value)
                {
                    loadAmount = value;
                    this.RaisePropertyChanged("LoadAmount");
                    if (loadAmount == true)
                    {
                        this.LoadAmountV = Visibility.Visible;
                    }
                    else
                    {
                        this.LoadAmountV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility loadAmountV = Visibility.Collapsed;//核定承载方量
        public Visibility LoadAmountV
        {
            get { return loadAmountV; }
            set
            {
                if (loadAmountV != value)
                {
                    loadAmountV = value;
                    this.RaisePropertyChanged("LoadAmountV");
                }
            }
        }

        private bool gPSID = false;//GPS车载终端编号
        public bool GPSID
        {
            get { return gPSID; }
            set
            {
                if (gPSID != value)
                {
                    gPSID = value;
                    this.RaisePropertyChanged("GPSID");
                    if (gPSID == true)
                    {
                        this.GPSIDV = Visibility.Visible;
                    }
                    else
                    {
                        this.GPSIDV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gPSIDV = Visibility.Collapsed;//GPS车载终端编号
        public Visibility GPSIDV
        {
            get { return gPSIDV; }
            set
            {
                if (gPSIDV != value)
                {
                    gPSIDV = value;
                    this.RaisePropertyChanged("GPSIDV");
                }
            }
        }

        private bool seatsNum = false;//座位数
        public bool SeatsNum
        {
            get { return seatsNum; }
            set
            {
                if (seatsNum != value)
                {
                    seatsNum = value;
                    this.RaisePropertyChanged("SeatsNum");
                    if (seatsNum == true)
                    {
                        this.SeatsNumV = Visibility.Visible;
                    }
                    else
                    {
                        this.SeatsNumV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility seatsNumV = Visibility.Collapsed;//座位数
        public Visibility SeatsNumV
        {
            get { return seatsNumV; }
            set
            {
                if (seatsNumV != value)
                {
                    seatsNumV = value;
                    this.RaisePropertyChanged("SeatsNumV");
                }
            }
        }

        private bool carDealers = false;//汽车销售商
        public bool CarDealers
        {
            get { return carDealers; }
            set
            {
                if (carDealers != value)
                {
                    carDealers = value;
                    this.RaisePropertyChanged("CarDealers");
                    if (carDealers == true)
                    {
                        this.CarDealersV = Visibility.Visible;
                    }
                    else
                    {
                        this.CarDealersV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility carDealersV = Visibility.Collapsed;//汽车销售商
        public Visibility CarDealersV
        {
            get { return carDealersV; }
            set
            {
                if (carDealersV != value)
                {
                    carDealersV = value;
                    this.RaisePropertyChanged("CarDealersV");
                }
            }
        }

        private bool purchaseDate = false;//购买日期
        public bool PurchaseDate
        {
            get { return purchaseDate; }
            set
            {
                if (purchaseDate != value)
                {
                    purchaseDate = value;
                    this.RaisePropertyChanged("PurchaseDate");
                    if (purchaseDate == true)
                    {
                        this.PurchaseDateV = Visibility.Visible;
                    }
                    else
                    {
                        this.PurchaseDateV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility purchaseDateV = Visibility.Collapsed;//购买日期
        public Visibility PurchaseDateV
        {
            get { return purchaseDateV; }
            set
            {
                if (purchaseDateV != value)
                {
                    purchaseDateV = value;
                    this.RaisePropertyChanged("PurchaseDateV");
                }
            }
        }

        private bool cardDate = false;//上牌日期
        public bool CardDate
        {
            get { return cardDate; }
            set
            {
                if (cardDate != value)
                {
                    cardDate = value;
                    this.RaisePropertyChanged("CardDate");
                    if (cardDate == true)
                    {
                        this.CardDateV = Visibility.Visible;
                    }
                    else
                    {
                        this.CardDateV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility cardDateV = Visibility.Collapsed;//上牌日期
        public Visibility CardDateV
        {
            get { return cardDateV; }
            set
            {
                if (cardDateV != value)
                {
                    cardDateV = value;
                    this.RaisePropertyChanged("CardDateV");
                }
            }
        }

        private bool debetOrNot = false;//是否贷保
        public bool DebetOrNot
        {
            get { return debetOrNot; }
            set
            {
                if (debetOrNot != value)
                {
                    debetOrNot = value;
                    this.RaisePropertyChanged("DebetOrNot");
                    if (debetOrNot == true)
                    {
                        this.DebetOrNotV = Visibility.Visible;
                    }
                    else
                    {
                        this.DebetOrNotV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility debetOrNotV = Visibility.Collapsed;//是否贷保
        public Visibility DebetOrNotV
        {
            get { return debetOrNotV; }
            set
            {
                if (debetOrNotV != value)
                {
                    debetOrNotV = value;
                    this.RaisePropertyChanged("DebetOrNotV");
                }
            }
        }

        private bool loadCapacity = false;//承载容量
        public bool LoadCapacity
        {
            get { return loadCapacity; }
            set
            {
                if (loadCapacity != value)
                {
                    loadCapacity = value;
                    this.RaisePropertyChanged("LoadCapacity");
                    if (loadCapacity == true)
                    {
                        this.LoadCapacityV = Visibility.Visible;
                    }
                    else
                    {
                        this.LoadCapacityV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility loadCapacityV = Visibility.Collapsed;//承载容量
        public Visibility LoadCapacityV
        {
            get { return loadCapacityV; }
            set
            {
                if (loadCapacityV != value)
                {
                    loadCapacityV = value;
                    this.RaisePropertyChanged("LoadCapacityV");
                }
            }
        }

        private bool fuelCapacity = false;//油箱容量
        public bool FuelCapacity
        {
            get { return fuelCapacity; }
            set
            {
                if (fuelCapacity != value)
                {
                    fuelCapacity = value;
                    this.RaisePropertyChanged("FuelCapacity");
                    if (fuelCapacity == true)
                    {
                        this.FuelCapacityV = Visibility.Visible;
                    }
                    else
                    {
                        this.FuelCapacityV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility fuelCapacityV = Visibility.Collapsed;//油箱容量
        public Visibility FuelCapacityV
        {
            get { return fuelCapacityV; }
            set
            {
                if (fuelCapacityV != value)
                {
                    fuelCapacityV = value;
                    this.RaisePropertyChanged("FuelCapacityV");
                }
            }
        }

        private bool loansOrNot = false;//是否贷款
        public bool LoansOrNot
        {
            get { return loansOrNot; }
            set
            {
                if (loansOrNot != value)
                {
                    loansOrNot = value;
                    this.RaisePropertyChanged("LoansOrNot");
                    if (loansOrNot == true)
                    {
                        this.LoansOrNotV = Visibility.Visible;
                    }
                    else
                    {
                        this.LoansOrNotV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility loansOrNotV = Visibility.Collapsed;//是否贷款
        public Visibility LoansOrNotV
        {
            get { return loansOrNotV; }
            set
            {
                if (loansOrNotV != value)
                {
                    loansOrNotV = value;
                    this.RaisePropertyChanged("LoansOrNotV");
                }
            }
        }

        private bool vehicleCosts = false;//车辆成本
        public bool VehicleCosts
        {
            get { return vehicleCosts; }
            set
            {
                if (vehicleCosts != value)
                {
                    vehicleCosts = value;
                    this.RaisePropertyChanged("VehicleCosts");
                    if (vehicleCosts == true)
                    {
                        this.VehicleCostsV = Visibility.Visible;
                    }
                    else
                    {
                        this.VehicleCostsV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vehicleCostsV = Visibility.Collapsed;//车辆成本
        public Visibility VehicleCostsV
        {
            get { return vehicleCostsV; }
            set
            {
                if (vehicleCostsV != value)
                {
                    vehicleCostsV = value;
                    this.RaisePropertyChanged("VehicleCostsV");
                }
            }
        }

        private bool life = false;//使用年限
        public bool Life
        {
            get { return life; }
            set
            {
                if (life != value)
                {
                    life = value;
                    this.RaisePropertyChanged("Life");
                    if (life == true)
                    {
                        this.LifeV = Visibility.Visible;
                    }
                    else
                    {
                        this.LifeV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility lifeV = Visibility.Collapsed;//使用年限
        public Visibility LifeV
        {
            get { return lifeV; }
            set
            {
                if (lifeV != value)
                {
                    lifeV = value;
                    this.RaisePropertyChanged("LifeV");
                }
            }
        }

        private bool salvageValue = false;//预计残值
        public bool SalvageValue
        {
            get { return salvageValue; }
            set
            {
                if (salvageValue != value)
                {
                    salvageValue = value;
                    this.RaisePropertyChanged("SalvageValue");
                    if (salvageValue == true)
                    {
                        this.SalvageValueV = Visibility.Visible;
                    }
                    else
                    {
                        this.SalvageValueV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility salvageValueV = Visibility.Collapsed;//预计残值
        public Visibility SalvageValueV
        {
            get { return salvageValueV; }
            set
            {
                if (salvageValueV != value)
                {
                    salvageValueV = value;
                    this.RaisePropertyChanged("SalvageValueV");
                }
            }
        }

        private bool vehicleState = true;//任务状况
        public bool VehicleState
        {
            get { return vehicleState; }
            set
            {
                if (vehicleState != value)
                {
                    vehicleState = value;
                    this.RaisePropertyChanged("VehicleState");
                    if (vehicleState == true)
                    {
                        this.VehicleStateV = Visibility.Visible;
                    }
                    else
                    {
                        this.VehicleStateV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vehicleStateV = Visibility.Visible;//任务状况
        public Visibility VehicleStateV
        {
            get { return vehicleStateV; }
            set
            {
                if (vehicleStateV != value)
                {
                    vehicleStateV = value;
                    this.RaisePropertyChanged("VehicleStateV");
                }
            }
        }
        #endregion

        #region gps基本信息
        private bool curLocation = true;//当前位置
        public bool CurLocation
        {
            get { return curLocation; }
            set
            {
                if (curLocation != value)
                {
                    curLocation = value;
                    this.RaisePropertyChanged("CurLocation");
                    if (curLocation == true)
                    {
                        this.CurLocationV = Visibility.Visible;
                    }
                    else
                    {
                        this.CurLocationV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility curLocationV = Visibility.Visible;//当前位置
        public Visibility CurLocationV
        {
            get { return curLocationV; }
            set
            {
                if (curLocationV != value)
                {
                    curLocationV = value;
                    this.RaisePropertyChanged("CurLocationV");
                }
            }
        }

        private bool longitude = true;//经度
        public bool Longitude
        {
            get { return longitude; }
            set
            {
                if (longitude != value)
                {
                    longitude = value;
                    this.RaisePropertyChanged("Longitude");
                    if (longitude == true)
                    {
                        this.LongitudeV = Visibility.Visible;
                    }
                    else
                    {
                        this.LongitudeV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility longitudeV = Visibility.Visible;//经度
        public Visibility LongitudeV
        {
            get { return longitudeV; }
            set
            {
                if (longitudeV != value)
                {
                    longitudeV = value;
                    this.RaisePropertyChanged("LongitudeV");
                }
            }
        }

        private bool latitude = true;//纬度
        public bool Latitude
        {
            get { return latitude; }
            set
            {
                if (latitude != value)
                {
                    latitude = value;
                    this.RaisePropertyChanged("Latitude");
                    if (latitude == true)
                    {
                        this.LatitudeV = Visibility.Visible;
                    }
                    else
                    {
                        this.LatitudeV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility latitudeV = Visibility.Visible;//纬度
        public Visibility LatitudeV
        {
            get { return latitudeV; }
            set
            {
                if (latitudeV != value)
                {
                    latitudeV = value;
                    this.RaisePropertyChanged("LatitudeV");
                }
            }
        }

        private bool direction = true;//表示方向
        public bool Direction
        {
            get { return direction; }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    this.RaisePropertyChanged("Direction");
                    if (direction == true)
                    {
                        this.DirectionV = Visibility.Visible;
                    }
                    else
                    {
                        this.DirectionV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility directionV = Visibility.Visible;//表示方向
        public Visibility DirectionV
        {
            get { return directionV; }
            set
            {
                if (directionV != value)
                {
                    directionV = value;
                    this.RaisePropertyChanged("DirectionV");
                }
            }
        }

        private bool gpsStatus = false;//GPS状态
        public bool GpsStatus
        {
            get { return gpsStatus; }
            set
            {
                if (gpsStatus != value)
                {
                    gpsStatus = value;
                    this.RaisePropertyChanged("GpsStatus");
                    if (gpsStatus == true)
                    {
                        this.GpsStatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.GpsStatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gpsStatusV = Visibility.Collapsed;//GPS状态
        public Visibility GpsStatusV
        {
            get { return gpsStatusV; }
            set
            {
                if (gpsStatusV != value)
                {
                    gpsStatusV = value;
                    this.RaisePropertyChanged("GpsStatusV");
                }
            }
        }

        private bool altitude = false;//高度
        public bool Altitude
        {
            get { return altitude; }
            set
            {
                if (altitude != value)
                {
                    altitude = value;
                    this.RaisePropertyChanged("Altitude");
                    if (altitude == true)
                    {
                        this.AltitudeV = Visibility.Visible;
                    }
                    else
                    {
                        this.AltitudeV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility altitudeV = Visibility.Collapsed;//高度
        public Visibility AltitudeV
        {
            get { return altitudeV; }
            set
            {
                if (altitudeV != value)
                {
                    altitudeV = value;
                    this.RaisePropertyChanged("AltitudeV");
                }
            }
        }

        private bool speed = true;//速度
        public bool Speed
        {
            get { return speed; }
            set
            {
                if (speed != value)
                {
                    speed = value;
                    this.RaisePropertyChanged("Speed");
                    if (speed == true)
                    {
                        this.SpeedV = Visibility.Visible;
                    }
                    else
                    {
                        this.SpeedV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility speedV = Visibility.Visible;//速度
        public Visibility SpeedV
        {
            get { return speedV; }
            set
            {
                if (speedV != value)
                {
                    speedV = value;
                    this.RaisePropertyChanged("SpeedV");
                }
            }
        }

        private bool devSpeed = true;//速度，行驶记录功能获取的速度
        public bool DevSpeed
        {
            get { return devSpeed; }
            set
            {
                if (devSpeed != value)
                {
                    devSpeed = value;
                    this.RaisePropertyChanged("DevSpeed");
                    if (devSpeed == true)
                    {
                        this.DevSpeedV = Visibility.Visible;
                    }
                    else
                    {
                        this.DevSpeedV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility devSpeedV = Visibility.Visible;//速度，行驶记录功能获取的速度
        public Visibility DevSpeedV
        {
            get { return devSpeedV; }
            set
            {
                if (devSpeedV != value)
                {
                    devSpeedV = value;
                    this.RaisePropertyChanged("DevSpeedV");
                }
            }
        }

        private bool mileage = true;//里程（对应车上里程表读数）
        public bool Mileage
        {
            get { return mileage; }
            set
            {
                if (mileage != value)
                {
                    mileage = value;
                    this.RaisePropertyChanged("Mileage");
                    if (mileage == true)
                    {
                        this.MileageV = Visibility.Visible;
                    }
                    else
                    {
                        this.MileageV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility mileageV = Visibility.Visible;//里程（对应车上里程表读数）
        public Visibility MileageV
        {
            get { return mileageV; }
            set
            {
                if (mileageV != value)
                {
                    mileageV = value;
                    this.RaisePropertyChanged("MileageV");
                }
            }
        }

        private bool gPSMileage = true;//里程（GPS读数） 
        public bool GPSMileage
        {
            get { return gPSMileage; }
            set
            {
                if (gPSMileage != value)
                {
                    gPSMileage = value;
                    this.RaisePropertyChanged("GPSMileage");
                    if (gPSMileage == true)
                    {
                        this.GPSMileageV = Visibility.Visible;
                    }
                    else
                    {
                        this.GPSMileageV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gPSMileageV = Visibility.Visible;//里程（GPS读数） 
        public Visibility GPSMileageV
        {
            get { return gPSMileageV; }
            set
            {
                if (gPSMileageV != value)
                {
                    gPSMileageV = value;
                    this.RaisePropertyChanged("GPSMileageV");
                }
            }
        }

        private bool oilVolumn = true;//油量（对应车上油量表读数）
        public bool OilVolumn
        {
            get { return oilVolumn; }
            set
            {
                if (oilVolumn != value)
                {
                    oilVolumn = value;
                    this.RaisePropertyChanged("OilVolumn");
                    if (oilVolumn == true)
                    {
                        this.OilVolumnV = Visibility.Visible;
                    }
                    else
                    {
                        this.OilVolumnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility oilVolumnV = Visibility.Visible;//油量（对应车上油量表读数）
        public Visibility OilVolumnV
        {
            get { return oilVolumnV; }
            set
            {
                if (oilVolumnV != value)
                {
                    oilVolumnV = value;
                    this.RaisePropertyChanged("OilVolumnV");
                }
            }
        }

        private bool datetime = true;//当前时间：YYYY-MM-DD HH-mm-ss
        public bool Datetime
        {
            get { return datetime; }
            set
            {
                if (datetime != value)
                {
                    datetime = value;
                    this.RaisePropertyChanged("Datetime");
                    if (datetime == true)
                    {
                        this.DatetimeV = Visibility.Visible;
                    }
                    else
                    {
                        this.DatetimeV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility datetimeV = Visibility.Visible;//当前时间：YYYY-MM-DD HH-mm-ss
        public Visibility DatetimeV
        {
            get { return datetimeV; }
            set
            {
                if (datetimeV != value)
                {
                    datetimeV = value;
                    this.RaisePropertyChanged("DatetimeV");
                }
            }
        }

        private bool onlineStates = true;//在线状态
        public bool OnlineStates
        {
            get { return onlineStates; }
            set
            {
                if (onlineStates != value)
                {
                    onlineStates = value;
                    this.RaisePropertyChanged("OnlineStates");
                    if (onlineStates == true)
                    {
                        this.OnlineStatesV = Visibility.Visible;
                    }
                    else
                    {
                        this.OnlineStatesV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility onlineStatesV = Visibility.Visible;//在线状态
        public Visibility OnlineStatesV
        {
            get { return onlineStatesV; }
            set
            {
                if (onlineStatesV != value)
                {
                    onlineStatesV = value;
                    this.RaisePropertyChanged("OnlineStatesV");
                }
            }
        }
        #endregion

        #region 车辆状态信息
        private bool accstatus = false;//ACC状态1开，0关
        public bool Accstatus
        {
            get { return accstatus; }
            set
            {
                if (accstatus != value)
                {
                    accstatus = value;
                    this.RaisePropertyChanged("Accstatus");
                    if (accstatus == true)
                    {
                        this.AccstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.AccstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility accstatusV = Visibility.Collapsed;//ACC状态1开，0关
        public Visibility AccstatusV
        {
            get { return accstatusV; }
            set
            {
                if (accstatusV != value)
                {
                    accstatusV = value;
                    this.RaisePropertyChanged("AccstatusV");
                }
            }
        }

        private bool workstatus = false;//运营状态：0运行，1停运
        public bool Workstatus
        {
            get { return workstatus; }
            set
            {
                if (workstatus != value)
                {
                    workstatus = value;
                    this.RaisePropertyChanged("Workstatus");
                    if (workstatus == true)
                    {
                        this.WorkstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.WorkstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility workstatusV = Visibility.Collapsed;//运营状态：0运行，1停运
        public Visibility WorkstatusV
        {
            get { return workstatusV; }
            set
            {
                if (workstatusV != value)
                {
                    workstatusV = value;
                    this.RaisePropertyChanged("WorkstatusV");
                }
            }
        }

        private bool llsecret = false;//经纬度加密状态
        public bool Llsecret
        {
            get { return llsecret; }
            set
            {
                if (llsecret != value)
                {
                    llsecret = value;
                    this.RaisePropertyChanged("Llsecret");
                    if (llsecret == true)
                    {
                        this.LlsecretV = Visibility.Visible;
                    }
                    else
                    {
                        this.LlsecretV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility llsecretV = Visibility.Collapsed;//经纬度加密状态
        public Visibility LlsecretV
        {
            get { return llsecretV; }
            set
            {
                if (llsecretV != value)
                {
                    llsecretV = value;
                    this.RaisePropertyChanged("LlsecretV");
                }
            }
        }

        private bool gpsmode = false;//GPS模式
        public bool Gpsmode
        {
            get { return gpsmode; }
            set
            {
                if (gpsmode != value)
                {
                    gpsmode = value;
                    this.RaisePropertyChanged("Gpsmode");
                    if (gpsmode == true)
                    {
                        this.GpsmodeV = Visibility.Visible;
                    }
                    else
                    {
                        this.GpsmodeV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gpsmodeV = Visibility.Collapsed;//GPS模式
        public Visibility GpsmodeV
        {
            get { return gpsmodeV; }
            set
            {
                if (gpsmodeV != value)
                {
                    gpsmodeV = value;
                    this.RaisePropertyChanged("GpsmodeV");
                }
            }
        }

        private bool oilwaystatus = false;//油路状态
        public bool Oilwaystatus
        {
            get { return oilwaystatus; }
            set
            {
                if (oilwaystatus != value)
                {
                    oilwaystatus = value;
                    this.RaisePropertyChanged("Oilwaystatus");
                    if (oilwaystatus == true)
                    {
                        this.OilwaystatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.OilwaystatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility oilwaystatusV = Visibility.Collapsed;//油路状态
        public Visibility OilwaystatusV
        {
            get { return oilwaystatusV; }
            set
            {
                if (oilwaystatusV != value)
                {
                    oilwaystatusV = value;
                    this.RaisePropertyChanged("OilwaystatusV");
                }
            }
        }

        private bool vcstatus = false;//车辆电路状态
        public bool Vcstatus
        {
            get { return vcstatus; }
            set
            {
                if (vcstatus != value)
                {
                    vcstatus = value;
                    this.RaisePropertyChanged("Vcstatus");
                    if (vcstatus == true)
                    {
                        this.VcstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.VcstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vcstatusV = Visibility.Collapsed;//车辆电路状态
        public Visibility VcstatusV
        {
            get { return vcstatusV; }
            set
            {
                if (vcstatusV != value)
                {
                    vcstatusV = value;
                    this.RaisePropertyChanged("VcstatusV");
                }
            }
        }

        private bool vdstatus = false;//车门状态
        public bool Vdstatus
        {
            get { return vdstatus; }
            set
            {
                if (vdstatus != value)
                {
                    vdstatus = value;
                    this.RaisePropertyChanged("Vdstatus");
                    if (vdstatus == true)
                    {
                        this.VdstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.VdstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vdstatusV = Visibility.Collapsed;//车门状态
        public Visibility VdstatusV
        {
            get { return vdstatusV; }
            set
            {
                if (vdstatusV != value)
                {
                    vdstatusV = value;
                    this.RaisePropertyChanged("VdstatusV");
                }
            }
        }

        private bool fdstatus = false;//前车门状态
        public bool Fdstatus
        {
            get { return fdstatus; }
            set
            {
                if (fdstatus != value)
                {
                    fdstatus = value;
                    this.RaisePropertyChanged("Fdstatus");
                    if (fdstatus == true)
                    {
                        this.FdstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.FdstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility fdstatusV = Visibility.Collapsed;//前车门状态
        public Visibility FdstatusV
        {
            get { return fdstatusV; }
            set
            {
                if (fdstatusV != value)
                {
                    fdstatusV = value;
                    this.RaisePropertyChanged("FdstatusV");
                }
            }
        }

        private bool bdstatus = false;//后车门状态
        public bool Bdstatus
        {
            get { return bdstatus; }
            set
            {
                if (bdstatus != value)
                {
                    bdstatus = value;
                    this.RaisePropertyChanged("Bdstatus");
                    if (bdstatus == true)
                    {
                        this.BdstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.BdstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility bdstatusV = Visibility.Collapsed;//后车门状态
        public Visibility BdstatusV
        {
            get { return bdstatusV; }
            set
            {
                if (bdstatusV != value)
                {
                    bdstatusV = value;
                    this.RaisePropertyChanged("BdstatusV");
                }
            }
        }

        private bool enginestatus = false;//发动机状态
        public bool Enginestatus
        {
            get { return enginestatus; }
            set
            {
                if (enginestatus != value)
                {
                    enginestatus = value;
                    this.RaisePropertyChanged("Enginestatus");
                    if (enginestatus == true)
                    {
                        this.EnginestatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.EnginestatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility enginestatusV = Visibility.Collapsed;//发动机状态
        public Visibility EnginestatusV
        {
            get { return enginestatusV; }
            set
            {
                if (enginestatusV != value)
                {
                    enginestatusV = value;
                    this.RaisePropertyChanged("EnginestatusV");
                }
            }
        }

        private bool conditionerstatus = false;//空调状态
        public bool Conditionerstatus
        {
            get { return conditionerstatus; }
            set
            {
                if (conditionerstatus != value)
                {
                    conditionerstatus = value;
                    this.RaisePropertyChanged("Conditionerstatus");
                    if (conditionerstatus == true)
                    {
                        this.ConditionerstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.ConditionerstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility conditionerstatusV = Visibility.Collapsed;//空调状态
        public Visibility ConditionerstatusV
        {
            get { return conditionerstatusV; }
            set
            {
                if (conditionerstatusV != value)
                {
                    conditionerstatusV = value;
                    this.RaisePropertyChanged("ConditionerstatusV");
                }
            }
        }

        private bool brakestatus = false;//刹车状态
        public bool Brakestatus
        {
            get { return brakestatus; }
            set
            {
                if (brakestatus != value)
                {
                    brakestatus = value;
                    this.RaisePropertyChanged("Brakestatus");
                    if (brakestatus == true)
                    {
                        this.BrakestatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.BrakestatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility brakestatusV = Visibility.Collapsed;//刹车状态
        public Visibility BrakestatusV
        {
            get { return brakestatusV; }
            set
            {
                if (brakestatusV != value)
                {
                    brakestatusV = value;
                    this.RaisePropertyChanged("BrakestatusV");
                }
            }
        }

        private bool ltstatus = false;//左转向状态
        public bool Ltstatus
        {
            get { return ltstatus; }
            set
            {
                if (ltstatus != value)
                {
                    ltstatus = value;
                    this.RaisePropertyChanged("Ltstatus");
                    if (ltstatus == true)
                    {
                        this.LtstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.LtstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility ltstatusV = Visibility.Collapsed;//左转向状态
        public Visibility LtstatusV
        {
            get { return ltstatusV; }
            set
            {
                if (ltstatusV != value)
                {
                    ltstatusV = value;
                    this.RaisePropertyChanged("LtstatusV");
                }
            }
        }

        private bool rtstatus = false;//右转向状态
        public bool Rtstatus
        {
            get { return rtstatus; }
            set
            {
                if (rtstatus != value)
                {
                    rtstatus = value;
                    this.RaisePropertyChanged("Rtstatus");
                    if (rtstatus == true)
                    {
                        this.RtstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.RtstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility rtstatusV = Visibility.Collapsed;//右转向状态
        public Visibility RtstatusV
        {
            get { return rtstatusV; }
            set
            {
                if (rtstatusV != value)
                {
                    rtstatusV = value;
                    this.RaisePropertyChanged("RtstatusV");
                }
            }
        }

        private bool farlstatus = false;//远光灯状态
        public bool Farlstatus
        {
            get { return farlstatus; }
            set
            {
                if (farlstatus != value)
                {
                    farlstatus = value;
                    this.RaisePropertyChanged("Farlstatus");
                    if (farlstatus == true)
                    {
                        this.FarlstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.FarlstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility farlstatusV = Visibility.Collapsed;//远光灯状态
        public Visibility FarlstatusV
        {
            get { return farlstatusV; }
            set
            {
                if (farlstatusV != value)
                {
                    farlstatusV = value;
                    this.RaisePropertyChanged("FarlstatusV");
                }
            }
        }

        private bool nearlstatus = false;//近光灯状态
        public bool Nearlstatus
        {
            get { return nearlstatus; }
            set
            {
                if (nearlstatus != value)
                {
                    nearlstatus = value;
                    this.RaisePropertyChanged("Nearlstatus");
                    if (nearlstatus == true)
                    {
                        this.NearlstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.NearlstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility nearlstatusV = Visibility.Collapsed;//近光灯状态
        public Visibility NearlstatusV
        {
            get { return nearlstatusV; }
            set
            {
                if (nearlstatusV != value)
                {
                    nearlstatusV = value;
                    this.RaisePropertyChanged("NearlstatusV");
                }
            }
        }

        private bool pnstatus = false;//正反转状态
        public bool Pnstatus
        {
            get { return pnstatus; }
            set
            {
                if (pnstatus != value)
                {
                    pnstatus = value;
                    this.RaisePropertyChanged("Pnstatus");
                    if (pnstatus == true)
                    {
                        this.PnstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.PnstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility pnstatusV = Visibility.Collapsed;//正反转状态
        public Visibility PnstatusV
        {
            get { return pnstatusV; }
            set
            {
                if (pnstatusV != value)
                {
                    pnstatusV = value;
                    this.RaisePropertyChanged("PnstatusV");
                }
            }
        }

        private bool shakestatus = false;//震动状态
        public bool Shakestatus
        {
            get { return shakestatus; }
            set
            {
                if (shakestatus != value)
                {
                    shakestatus = value;
                    this.RaisePropertyChanged("Shakestatus");
                    if (shakestatus == true)
                    {
                        this.ShakestatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.ShakestatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility shakestatusV = Visibility.Collapsed;//震动状态
        public Visibility ShakestatusV
        {
            get { return shakestatusV; }
            set
            {
                if (shakestatusV != value)
                {
                    shakestatusV = value;
                    this.RaisePropertyChanged("ShakestatusV");
                }
            }
        }

        private bool hornstatus = false;//喇叭状态
        public bool Hornstatus
        {
            get { return hornstatus; }
            set
            {
                if (hornstatus != value)
                {
                    hornstatus = value;
                    this.RaisePropertyChanged("Hornstatus");
                    if (hornstatus == true)
                    {
                        this.HornstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.HornstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility hornstatusV = Visibility.Collapsed;//喇叭状态
        public Visibility HornstatusV
        {
            get { return hornstatusV; }
            set
            {
                if (hornstatusV != value)
                {
                    hornstatusV = value;
                    this.RaisePropertyChanged("HornstatusV");
                }
            }
        }

        private bool protectstatus = false;//安全状态
        public bool Protectstatus
        {
            get { return protectstatus; }
            set
            {
                if (protectstatus != value)
                {
                    protectstatus = value;
                    this.RaisePropertyChanged("Protectstatus");
                    if (protectstatus == true)
                    {
                        this.ProtectstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.ProtectstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility protectstatusV = Visibility.Collapsed;//安全状态
        public Visibility ProtectstatusV
        {
            get { return protectstatusV; }
            set
            {
                if (protectstatusV != value)
                {
                    protectstatusV = value;
                    this.RaisePropertyChanged("ProtectstatusV");
                }
            }
        }

        private bool loadstatus = false;//负载状态
        public bool Loadstatus
        {
            get { return loadstatus; }
            set
            {
                if (loadstatus != value)
                {
                    loadstatus = value;
                    this.RaisePropertyChanged("Loadstatus");
                    if (loadstatus == true)
                    {
                        this.LoadstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.LoadstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility loadstatusV = Visibility.Collapsed;//负载状态
        public Visibility LoadstatusV
        {
            get { return loadstatusV; }
            set
            {
                if (loadstatusV != value)
                {
                    loadstatusV = value;
                    this.RaisePropertyChanged("LoadstatusV");
                }
            }
        }

        private bool busstatus = false;//总线状态
        public bool Busstatus
        {
            get { return busstatus; }
            set
            {
                if (busstatus != value)
                {
                    busstatus = value;
                    this.RaisePropertyChanged("Busstatus");
                    if (busstatus == true)
                    {
                        this.BusstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.BusstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility busstatusV = Visibility.Collapsed;//总线状态
        public Visibility BusstatusV
        {
            get { return busstatusV; }
            set
            {
                if (busstatusV != value)
                {
                    busstatusV = value;
                    this.RaisePropertyChanged("BusstatusV");
                }
            }
        }

        private bool gsmstatus = false;//GSM模块状态
        public bool Gsmstatus
        {
            get { return gsmstatus; }
            set
            {
                if (gsmstatus != value)
                {
                    gsmstatus = value;
                    this.RaisePropertyChanged("Gsmstatus");
                    if (gsmstatus == true)
                    {
                        this.GsmstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.GsmstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gsmstatusV = Visibility.Collapsed;//GSM模块状态
        public Visibility GsmstatusV
        {
            get { return gsmstatusV; }
            set
            {
                if (gsmstatusV != value)
                {
                    gsmstatusV = value;
                    this.RaisePropertyChanged("GsmstatusV");
                }
            }
        }

        private bool gpsstatus = false;//GPS模块状态
        public bool Gpsstatus
        {
            get { return gpsstatus; }
            set
            {
                if (gpsstatus != value)
                {
                    gpsstatus = value;
                    this.RaisePropertyChanged("Gpsstatus");
                    if (gpsstatus == true)
                    {
                        this.GpsstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.GpsstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gpsstatusV = Visibility.Collapsed;//GPS模块状态
        public Visibility GpsstatusV
        {
            get { return gpsstatusV; }
            set
            {
                if (gpsstatusV != value)
                {
                    gpsstatusV = value;
                    this.RaisePropertyChanged("GpsstatusV");
                }
            }
        }

        private bool lcstatus = false;//锁车电路状态
        public bool Lcstatus
        {
            get { return lcstatus; }
            set
            {
                if (lcstatus != value)
                {
                    lcstatus = value;
                    this.RaisePropertyChanged("Lcstatus");
                    if (lcstatus == true)
                    {
                        this.LcstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.LcstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility lcstatusV = Visibility.Collapsed;//锁车电路状态
        public Visibility LcstatusV
        {
            get { return lcstatusV; }
            set
            {
                if (lcstatusV != value)
                {
                    lcstatusV = value;
                    this.RaisePropertyChanged("LcstatusV");
                }
            }
        }

        private bool ffstatus = false;//前雾灯状态
        public bool Ffstatus
        {
            get { return ffstatus; }
            set
            {
                if (ffstatus != value)
                {
                    ffstatus = value;
                    this.RaisePropertyChanged("Ffstatus");
                    if (ffstatus == true)
                    {
                        this.FfstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.FfstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility ffstatusV = Visibility.Collapsed;//前雾灯状态
        public Visibility FfstatusV
        {
            get { return ffstatusV; }
            set
            {
                if (ffstatusV != value)
                {
                    ffstatusV = value;
                    this.RaisePropertyChanged("FfstatusV");
                }
            }
        }

        private bool bfstatus = false;//后雾灯状态
        public bool Bfstatus
        {
            get { return bfstatus; }
            set
            {
                if (bfstatus != value)
                {
                    bfstatus = value;
                    this.RaisePropertyChanged("Bfstatus");
                    if (bfstatus == true)
                    {
                        this.BfstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.BfstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility bfstatusV = Visibility.Collapsed;//后雾灯状态
        public Visibility BfstatusV
        {
            get { return bfstatusV; }
            set
            {
                if (bfstatusV != value)
                {
                    bfstatusV = value;
                    this.RaisePropertyChanged("BfstatusV");
                }
            }
        }

        private bool gpsantstatus = false;//GPS天线状态
        public bool Gpsantstatus
        {
            get { return gpsantstatus; }
            set
            {
                if (gpsantstatus != value)
                {
                    gpsantstatus = value;
                    this.RaisePropertyChanged("Gpsantstatus");
                    if (gpsantstatus == true)
                    {
                        this.GpsantstatusV = Visibility.Visible;
                    }
                    else
                    {
                        this.GpsantstatusV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gpsantstatusV = Visibility.Collapsed;//GPS天线状态
        public Visibility GpsantstatusV
        {
            get { return gpsantstatusV; }
            set
            {
                if (gpsantstatusV != value)
                {
                    gpsantstatusV = value;
                    this.RaisePropertyChanged("GpsantstatusV");
                }
            }
        }
        #endregion

        #region 车辆报警状态
        private bool soswarn = false;//紧急报警，触动报警开关后触发
        public bool Soswarn
        {
            get { return soswarn; }
            set
            {
                if (soswarn != value)
                {
                    soswarn = value;
                    this.RaisePropertyChanged("Soswarn");
                    if (soswarn == true)
                    {
                        this.SoswarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.SoswarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility soswarnV = Visibility.Collapsed;//紧急报警，触动报警开关后触发
        public Visibility SoswarnV
        {
            get { return soswarnV; }
            set
            {
                if (soswarnV != value)
                {
                    soswarnV = value;
                    this.RaisePropertyChanged("SoswarnV");
                }
            }
        }

        private bool overspeedwarn = false;//超速报警
        public bool Overspeedwarn
        {
            get { return overspeedwarn; }
            set
            {
                if (overspeedwarn != value)
                {
                    overspeedwarn = value;
                    this.RaisePropertyChanged("Overspeedwarn");
                    if (overspeedwarn == true)
                    {
                        this.OverspeedwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.OverspeedwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility overspeedwarnV = Visibility.Collapsed;//超速报警
        public Visibility OverspeedwarnV
        {
            get { return overspeedwarnV; }
            set
            {
                if (overspeedwarnV != value)
                {
                    overspeedwarnV = value;
                    this.RaisePropertyChanged("OverspeedwarnV");
                }
            }
        }

        private bool tiredwarn = false;//疲劳驾驶
        public bool Tiredwarn
        {
            get { return tiredwarn; }
            set
            {
                if (tiredwarn != value)
                {
                    tiredwarn = value;
                    this.RaisePropertyChanged("Tiredwarn");
                    if (tiredwarn == true)
                    {
                        this.TiredwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.TiredwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility tiredwarnV = Visibility.Collapsed;//疲劳驾驶
        public Visibility TiredwarnV
        {
            get { return tiredwarnV; }
            set
            {
                if (tiredwarnV != value)
                {
                    tiredwarnV = value;
                    this.RaisePropertyChanged("TiredwarnV");
                }
            }
        }

        private bool prewarn = false;//预警
        public bool Prewarn
        {
            get { return prewarn; }
            set
            {
                if (prewarn != value)
                {
                    prewarn = value;
                    this.RaisePropertyChanged("Prewarn");
                    if (prewarn == true)
                    {
                        this.PrewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.PrewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility prewarnV = Visibility.Collapsed;//预警
        public Visibility PrewarnV
        {
            get { return prewarnV; }
            set
            {
                if (prewarnV != value)
                {
                    prewarnV = value;
                    this.RaisePropertyChanged("PrewarnV");
                }
            }
        }

        private bool gnssfatal = false;//GNSS模块故障
        public bool Gnssfatal
        {
            get { return gnssfatal; }
            set
            {
                if (gnssfatal != value)
                {
                    gnssfatal = value;
                    this.RaisePropertyChanged("Gnssfatal");
                    if (gnssfatal == true)
                    {
                        this.GnssfatalV = Visibility.Visible;
                    }
                    else
                    {
                        this.GnssfatalV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gnssfatalV = Visibility.Collapsed;//GNSS模块故障
        public Visibility GnssfatalV
        {
            get { return gnssfatalV; }
            set
            {
                if (gnssfatalV != value)
                {
                    gnssfatalV = value;
                    this.RaisePropertyChanged("GnssfatalV");
                }
            }
        }

        private bool gnssantwarn = false;//GNSS天线未接或被剪断
        public bool Gnssantwarn
        {
            get { return gnssantwarn; }
            set
            {
                if (gnssantwarn != value)
                {
                    gnssantwarn = value;
                    this.RaisePropertyChanged("Gnssantwarn");
                    if (gnssantwarn == true)
                    {
                        this.GnssantwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.GnssantwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gnssantwarnV = Visibility.Collapsed;
        public Visibility GnssantwarnV
        {
            get { return gnssantwarnV; }
            set
            {
                if (gnssantwarnV != value)
                {
                    gnssantwarnV = value;
                    this.RaisePropertyChanged("GnssantwarnV");
                }
            }
        }

        private bool gnssshortwarn = false;//GNSS天线短路
        public bool Gnssshortwarn
        {
            get { return gnssshortwarn; }
            set
            {
                if (gnssshortwarn != value)
                {
                    gnssshortwarn = value;
                    this.RaisePropertyChanged("Gnssshortwarn");
                    if (gnssshortwarn == true)
                    {
                        this.GnssshortwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.GnssshortwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility gnssshortwarnV = Visibility.Collapsed;//GNSS天线短路
        public Visibility GnssshortwarnV
        {
            get { return gnssshortwarnV; }
            set
            {
                if (gnssshortwarnV != value)
                {
                    gnssshortwarnV = value;
                    this.RaisePropertyChanged("GnssshortwarnV");
                }
            }
        }

        private bool lowvolwarn = false;//终端主电源欠压
        public bool Lowvolwarn
        {
            get { return lowvolwarn; }
            set
            {
                if (lowvolwarn != value)
                {
                    lowvolwarn = value;
                    this.RaisePropertyChanged("Lowvolwarn");
                    if (lowvolwarn == true)
                    {
                        this.LowvolwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.LowvolwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility lowvolwarnV = Visibility.Collapsed;//终端主电源欠压
        public Visibility LowvolwarnV
        {
            get { return lowvolwarnV; }
            set
            {
                if (lowvolwarnV != value)
                {
                    lowvolwarnV = value;
                    this.RaisePropertyChanged("LowvolwarnV");
                }
            }
        }

        private bool highvolwarn = false;//终端主电源高压
        public bool Highvolwarn
        {
            get { return highvolwarn; }
            set
            {
                if (highvolwarn != value)
                {
                    highvolwarn = value;
                    this.RaisePropertyChanged("Highvolwarn");
                    if (highvolwarn == true)
                    {
                        this.HighvolwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.HighvolwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility highvolwarnV = Visibility.Collapsed;//终端主电源高压
        public Visibility HighvolwarnV
        {
            get { return highvolwarnV; }
            set
            {
                if (highvolwarnV != value)
                {
                    highvolwarnV = value;
                    this.RaisePropertyChanged("HighvolwarnV");
                }
            }
        }

        private bool outagewarn = false;//终端主电源断电
        public bool Outagewarn
        {
            get { return outagewarn; }
            set
            {
                if (outagewarn != value)
                {
                    outagewarn = value;
                    this.RaisePropertyChanged("Outagewarn");
                    if (outagewarn == true)
                    {
                        this.OutagewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.OutagewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility outagewarnV = Visibility.Collapsed;//终端主电源断电
        public Visibility OutagewarnV
        {
            get { return outagewarnV; }
            set
            {
                if (outagewarnV != value)
                {
                    outagewarnV = value;
                    this.RaisePropertyChanged("OutagewarnV");
                }
            }
        }

        private bool lcdfatalwarn = false;//终端LCD或者显示器故障
        public bool Lcdfatalwarn
        {
            get { return lcdfatalwarn; }
            set
            {
                if (lcdfatalwarn != value)
                {
                    lcdfatalwarn = value;
                    this.RaisePropertyChanged("Lcdfatalwarn");
                    if (lcdfatalwarn == true)
                    {
                        this.LcdfatalwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.LcdfatalwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility lcdfatalwarnV = Visibility.Collapsed;//终端LCD或者显示器故障
        public Visibility LcdfatalwarnV
        {
            get { return lcdfatalwarnV; }
            set
            {
                if (lcdfatalwarnV != value)
                {
                    lcdfatalwarnV = value;
                    this.RaisePropertyChanged("LcdfatalwarnV");
                }
            }
        }

        private bool ttsfatalwarn = false;//TTS模块故障
        public bool Ttsfatalwarn
        {
            get { return ttsfatalwarn; }
            set
            {
                if (ttsfatalwarn != value)
                {
                    ttsfatalwarn = value;
                    this.RaisePropertyChanged("Ttsfatalwarn");
                    if (ttsfatalwarn == true)
                    {
                        this.TtsfatalwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.TtsfatalwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility ttsfatalwarnV = Visibility.Collapsed;//TTS模块故障
        public Visibility TtsfatalwarnV
        {
            get { return ttsfatalwarnV; }
            set
            {
                if (ttsfatalwarnV != value)
                {
                    ttsfatalwarnV = value;
                    this.RaisePropertyChanged("TtsfatalwarnV");
                }
            }
        }

        private bool camerafatalwarn = false;//摄像头故障
        public bool Camerafatalwarn
        {
            get { return camerafatalwarn; }
            set
            {
                if (camerafatalwarn != value)
                {
                    camerafatalwarn = value;
                    this.RaisePropertyChanged("Camerafatalwarn");
                    if (camerafatalwarn == true)
                    {
                        this.CamerafatalwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.CamerafatalwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility camerafatalwarnV = Visibility.Collapsed;//摄像头故障
        public Visibility CamerafatalwarnV
        {
            get { return camerafatalwarnV; }
            set
            {
                if (camerafatalwarnV != value)
                {
                    camerafatalwarnV = value;
                    this.RaisePropertyChanged("CamerafatalwarnV");
                }
            }
        }

        private bool vediolosewarn = false;//视频丢失报警
        public bool Vediolosewarn
        {
            get { return vediolosewarn; }
            set
            {
                if (vediolosewarn != value)
                {
                    vediolosewarn = value;
                    this.RaisePropertyChanged("Vediolosewarn");
                    if (vediolosewarn == true)
                    {
                        this.VediolosewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.VediolosewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vediolosewarnV = Visibility.Collapsed;//视频丢失报警
        public Visibility VediolosewarnV
        {
            get { return vediolosewarnV; }
            set
            {
                if (vediolosewarnV != value)
                {
                    vediolosewarnV = value;
                    this.RaisePropertyChanged("VediolosewarnV");
                }
            }
        }

        private bool vedioshelterwarn = false;//视频遮挡报警
        public bool Vedioshelterwarn
        {
            get { return vedioshelterwarn; }
            set
            {
                if (vedioshelterwarn != value)
                {
                    vedioshelterwarn = value;
                    this.RaisePropertyChanged("Vedioshelterwarn");
                    if (vedioshelterwarn == true)
                    {
                        this.VedioshelterwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.VedioshelterwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vedioshelterwarnV = Visibility.Collapsed;//视频遮挡报警
        public Visibility VedioshelterwarnV
        {
            get { return vedioshelterwarnV; }
            set
            {
                if (vedioshelterwarnV != value)
                {
                    vedioshelterwarnV = value;
                    this.RaisePropertyChanged("VedioshelterwarnV");
                }
            }
        }

        private bool accumtimeout = false;//当天累计驾驶超时
        public bool Accumtimeout
        {
            get { return accumtimeout; }
            set
            {
                if (accumtimeout != value)
                {
                    accumtimeout = value;
                    this.RaisePropertyChanged("Accumtimeout");
                    if (accumtimeout == true)
                    {
                        this.AccumtimeoutV = Visibility.Visible;
                    }
                    else
                    {
                        this.AccumtimeoutV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility accumtimeoutV = Visibility.Collapsed;//当天累计驾驶超时
        public Visibility AccumtimeoutV
        {
            get { return accumtimeoutV; }
            set
            {
                if (accumtimeoutV != value)
                {
                    accumtimeoutV = value;
                    this.RaisePropertyChanged("AccumtimeoutV");
                }
            }
        }

        private bool stoptimeout = false;//超时停车
        public bool Stoptimeout
        {
            get { return stoptimeout; }
            set
            {
                if (stoptimeout != value)
                {
                    stoptimeout = value;
                    this.RaisePropertyChanged("Stoptimeout");
                    if (stoptimeout == true)
                    {
                        this.StoptimeoutV = Visibility.Visible;
                    }
                    else
                    {
                        this.StoptimeoutV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility stoptimeoutV = Visibility.Collapsed;//超时停车
        public Visibility StoptimeoutV
        {
            get { return stoptimeoutV; }
            set
            {
                if (stoptimeoutV != value)
                {
                    stoptimeoutV = value;
                    this.RaisePropertyChanged("StoptimeoutV");
                }
            }
        }

        private bool inoutareawarn = false;//进出区域报警
        public bool Inoutareawarn
        {
            get { return inoutareawarn; }
            set
            {
                if (inoutareawarn != value)
                {
                    inoutareawarn = value;
                    this.RaisePropertyChanged("Inoutareawarn");
                    if (inoutareawarn == true)
                    {
                        this.InoutareawarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.InoutareawarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility inoutareawarnV = Visibility.Collapsed;//进出区域报警
        public Visibility InoutareawarnV
        {
            get { return inoutareawarnV; }
            set
            {
                if (inoutareawarnV != value)
                {
                    inoutareawarnV = value;
                    this.RaisePropertyChanged("InoutareawarnV");
                }
            }
        }

        private bool inoutlinewarn = false;//进出录像报警
        public bool Inoutlinewarn
        {
            get { return inoutlinewarn; }
            set
            {
                if (inoutlinewarn != value)
                {
                    inoutlinewarn = value;
                    this.RaisePropertyChanged("Inoutlinewarn");
                    if (inoutlinewarn == true)
                    {
                        this.InoutlinewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.InoutlinewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility inoutlinewarnV = Visibility.Collapsed;//进出录像报警
        public Visibility InoutlinewarnV
        {
            get { return inoutlinewarnV; }
            set
            {
                if (inoutlinewarnV != value)
                {
                    inoutlinewarnV = value;
                    this.RaisePropertyChanged("InoutlinewarnV");
                }
            }
        }

        private bool drivingtimewarn = false;//路段行驶时间不足/过长报警
        public bool Drivingtimewarn
        {
            get { return drivingtimewarn; }
            set
            {
                if (drivingtimewarn != value)
                {
                    drivingtimewarn = value;
                    this.RaisePropertyChanged("Drivingtimewarn");
                    if (drivingtimewarn == true)
                    {
                        this.DrivingtimewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.DrivingtimewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility drivingtimewarnV = Visibility.Collapsed;//路段行驶时间不足/过长报警
        public Visibility DrivingtimewarnV
        {
            get { return drivingtimewarnV; }
            set
            {
                if (drivingtimewarnV != value)
                {
                    drivingtimewarnV = value;
                    this.RaisePropertyChanged("DrivingtimewarnV");
                }
            }
        }

        private bool deviatewarn = false;//路线偏离报警
        public bool Deviatewarn
        {
            get { return deviatewarn; }
            set
            {
                if (deviatewarn != value)
                {
                    deviatewarn = value;
                    this.RaisePropertyChanged("Deviatewarn");
                    if (deviatewarn == true)
                    {
                        this.DeviatewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.DeviatewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility deviatewarnV = Visibility.Collapsed;//路线偏离报警
        public Visibility DeviatewarnV
        {
            get { return deviatewarnV; }
            set
            {
                if (deviatewarnV != value)
                {
                    deviatewarnV = value;
                    this.RaisePropertyChanged("DeviatewarnV");
                }
            }
        }

        private bool vssfatalwarn = false;//车辆VSS故障
        public bool Vssfatalwarn
        {
            get { return vssfatalwarn; }
            set
            {
                if (vssfatalwarn != value)
                {
                    vssfatalwarn = value;
                    this.RaisePropertyChanged("Vssfatalwarn");
                    if (vssfatalwarn == true)
                    {
                        this.VssfatalwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.VssfatalwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vssfatalwarnV = Visibility.Collapsed;//车辆VSS故障
        public Visibility VssfatalwarnV
        {
            get { return vssfatalwarnV; }
            set
            {
                if (vssfatalwarnV != value)
                {
                    vssfatalwarnV = value;
                    this.RaisePropertyChanged("VssfatalwarnV");
                }
            }
        }

        private bool oilexceptionwarn = false;//车辆油量异常报警
        public bool Oilexceptionwarn
        {
            get { return oilexceptionwarn; }
            set
            {
                if (oilexceptionwarn != value)
                {
                    oilexceptionwarn = value;
                    this.RaisePropertyChanged("Oilexceptionwarn");
                    if (oilexceptionwarn == true)
                    {
                        this.OilexceptionwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.OilexceptionwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility oilexceptionwarnV = Visibility.Collapsed;//车辆油量异常报警
        public Visibility OilexceptionwarnV
        {
            get { return oilexceptionwarnV; }
            set
            {
                if (oilexceptionwarnV != value)
                {
                    oilexceptionwarnV = value;
                    this.RaisePropertyChanged("OilexceptionwarnV");
                }
            }
        }

        private bool vehiclestolenwarn = false;//车辆被盗报警
        public bool Vehiclestolenwarn
        {
            get { return vehiclestolenwarn; }
            set
            {
                if (vehiclestolenwarn != value)
                {
                    vehiclestolenwarn = value;
                    this.RaisePropertyChanged("Vehiclestolenwarn");
                    if (vehiclestolenwarn == true)
                    {
                        this.VehiclestolenwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.VehiclestolenwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility vehiclestolenwarnV = Visibility.Collapsed;//车辆被盗报警
        public Visibility VehiclestolenwarnV
        {
            get { return vehiclestolenwarnV; }
            set
            {
                if (vehiclestolenwarnV != value)
                {
                    vehiclestolenwarnV = value;
                    this.RaisePropertyChanged("VehiclestolenwarnV");
                }
            }
        }

        private bool illignitewarn = false;//非法点火报警
        public bool Illignitewarn
        {
            get { return illignitewarn; }
            set
            {
                if (illignitewarn != value)
                {
                    illignitewarn = value;
                    this.RaisePropertyChanged("Illignitewarn");
                    if (illignitewarn == true)
                    {
                        this.IllignitewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.IllignitewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility illignitewarnV = Visibility.Collapsed;//非法点火报警
        public Visibility IllignitewarnV
        {
            get { return illignitewarnV; }
            set
            {
                if (illignitewarnV != value)
                {
                    illignitewarnV = value;
                    this.RaisePropertyChanged("IllignitewarnV");
                }
            }
        }

        private bool illmovewarn = false;//非法位移报警
        public bool Illmovewarn
        {
            get { return illmovewarn; }
            set
            {
                if (illmovewarn != value)
                {
                    illmovewarn = value;
                    this.RaisePropertyChanged("Illmovewarn");
                    if (illmovewarn == true)
                    {
                        this.IllmovewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.IllmovewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility illmovewarnV = Visibility.Collapsed;//非法位移报警
        public Visibility IllmovewarnV
        {
            get { return illmovewarnV; }
            set
            {
                if (illmovewarnV != value)
                {
                    illmovewarnV = value;
                    this.RaisePropertyChanged("IllmovewarnV");
                }
            }
        }

        private bool crashwarn = false;//碰撞侧翻报警
        public bool Crashwarn
        {
            get { return crashwarn; }
            set
            {
                if (crashwarn != value)
                {
                    crashwarn = value;
                    this.RaisePropertyChanged("Crashwarn");
                    if (crashwarn == true)
                    {
                        this.CrashwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.CrashwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility crashwarnV = Visibility.Collapsed;//碰撞侧翻报警
        public Visibility CrashwarnV
        {
            get { return crashwarnV; }
            set
            {
                if (crashwarnV != value)
                {
                    crashwarnV = value;
                    this.RaisePropertyChanged("CrashwarnV");
                }
            }
        }

        private bool sdexceptionwarn = false;//SD卡异常报警
        public bool Sdexceptionwarn
        {
            get { return sdexceptionwarn; }
            set
            {
                if (sdexceptionwarn != value)
                {
                    sdexceptionwarn = value;
                    this.RaisePropertyChanged("Sdexceptionwarn");
                    if (sdexceptionwarn == true)
                    {
                        this.SdexceptionwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.SdexceptionwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility sdexceptionwarnV = Visibility.Collapsed;//SD卡异常报警
        public Visibility SdexceptionwarnV
        {
            get { return sdexceptionwarnV; }
            set
            {
                if (sdexceptionwarnV != value)
                {
                    sdexceptionwarnV = value;
                    this.RaisePropertyChanged("SdexceptionwarnV");
                }
            }
        }

        private bool robwarn = false;//劫警
        public bool Robwarn
        {
            get { return robwarn; }
            set
            {
                if (robwarn != value)
                {
                    robwarn = value;
                    this.RaisePropertyChanged("Robwarn");
                    if (robwarn == true)
                    {
                        this.RobwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.RobwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility robwarnV = Visibility.Collapsed;//劫警
        public Visibility RobwarnV
        {
            get { return robwarnV; }
            set
            {
                if (robwarnV != value)
                {
                    robwarnV = value;
                    this.RaisePropertyChanged("RobwarnV");
                }
            }
        }

        private bool sleeptimewarn = false;//司机停车休息时间不足报警
        public bool Sleeptimewarn
        {
            get { return sleeptimewarn; }
            set
            {
                if (sleeptimewarn != value)
                {
                    sleeptimewarn = value;
                    this.RaisePropertyChanged("Sleeptimewarn");
                    if (sleeptimewarn == true)
                    {
                        this.SleeptimewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.SleeptimewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility sleeptimewarnV = Visibility.Collapsed;//司机停车休息时间不足报警
        public Visibility SleeptimewarnV
        {
            get { return sleeptimewarnV; }
            set
            {
                if (sleeptimewarnV != value)
                {
                    sleeptimewarnV = value;
                    this.RaisePropertyChanged("SleeptimewarnV");
                }
            }
        }

        private bool illtimedrivingwarn = false;//非法时段行驶报警
        public bool Illtimedrivingwarn
        {
            get { return illtimedrivingwarn; }
            set
            {
                if (illtimedrivingwarn != value)
                {
                    illtimedrivingwarn = value;
                    this.RaisePropertyChanged("Illtimedrivingwarn");
                    if (illtimedrivingwarn == true)
                    {
                        this.IlltimedrivingwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.IlltimedrivingwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility illtimedrivingwarnV = Visibility.Collapsed;//视频丢失报警
        public Visibility IlltimedrivingwarnV
        {
            get { return illtimedrivingwarnV; }
            set
            {
                if (illtimedrivingwarnV != value)
                {
                    illtimedrivingwarnV = value;
                    this.RaisePropertyChanged("IlltimedrivingwarnV");
                }
            }
        }

        private bool overstationwarn = false;//越战报警
        public bool Overstationwarn
        {
            get { return overstationwarn; }
            set
            {
                if (overstationwarn != value)
                {
                    overstationwarn = value;
                    this.RaisePropertyChanged("Overstationwarn");
                    if (overstationwarn == true)
                    {
                        this.OverstationwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.OverstationwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility overstationwarnV = Visibility.Collapsed;//越战报警
        public Visibility OverstationwarnV
        {
            get { return overstationwarnV; }
            set
            {
                if (overstationwarnV != value)
                {
                    overstationwarnV = value;
                    this.RaisePropertyChanged("OverstationwarnV");
                }
            }
        }

        private bool ilopendoorwarn = false;//非法开车门报警
        public bool Ilopendoorwarn
        {
            get { return ilopendoorwarn; }
            set
            {
                if (ilopendoorwarn != value)
                {
                    ilopendoorwarn = value;
                    this.RaisePropertyChanged("Ilopendoorwarn");
                    if (ilopendoorwarn == true)
                    {
                        this.IlopendoorwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.IlopendoorwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility ilopendoorwarnV = Visibility.Collapsed;//非法开车门报警
        public Visibility IlopendoorwarnV
        {
            get { return ilopendoorwarnV; }
            set
            {
                if (ilopendoorwarnV != value)
                {
                    ilopendoorwarnV = value;
                    this.RaisePropertyChanged("IlopendoorwarnV");
                }
            }
        }

        private bool protectwarn = false;//设防报警
        public bool Protectwarn
        {
            get { return protectwarn; }
            set
            {
                if (protectwarn != value)
                {
                    protectwarn = value;
                    this.RaisePropertyChanged("Protectwarn");
                    if (protectwarn == true)
                    {
                        this.ProtectwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.ProtectwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility protectwarnV = Visibility.Collapsed;//设防报警
        public Visibility ProtectwarnV
        {
            get { return protectwarnV; }
            set
            {
                if (protectwarnV != value)
                {
                    protectwarnV = value;
                    this.RaisePropertyChanged("ProtectwarnV");
                }
            }
        }

        private bool trimmingwarn = false;//剪线报警
        public bool Trimmingwarn
        {
            get { return trimmingwarn; }
            set
            {
                if (trimmingwarn != value)
                {
                    trimmingwarn = value;
                    this.RaisePropertyChanged("Trimmingwarn");
                    if (trimmingwarn == true)
                    {
                        this.TrimmingwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.TrimmingwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility trimmingwarnV = Visibility.Collapsed;//剪线报警
        public Visibility TrimmingwarnV
        {
            get { return trimmingwarnV; }
            set
            {
                if (trimmingwarnV != value)
                {
                    trimmingwarnV = value;
                    this.RaisePropertyChanged("TrimmingwarnV");
                }
            }
        }

        private bool passwdwarn = false;//密码错误报警
        public bool Passwdwarn
        {
            get { return passwdwarn; }
            set
            {
                if (passwdwarn != value)
                {
                    passwdwarn = value;
                    this.RaisePropertyChanged("Passwdwarn");
                    if (passwdwarn == true)
                    {
                        this.PasswdwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.PasswdwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility passwdwarnV = Visibility.Collapsed;//密码错误报警
        public Visibility PasswdwarnV
        {
            get { return passwdwarnV; }
            set
            {
                if (passwdwarnV != value)
                {
                    passwdwarnV = value;
                    this.RaisePropertyChanged("PasswdwarnV");
                }
            }
        }

        private bool prohibitmovewarn = false;//禁行报警
        public bool Prohibitmovewarn
        {
            get { return prohibitmovewarn; }
            set
            {
                if (prohibitmovewarn != value)
                {
                    prohibitmovewarn = value;
                    this.RaisePropertyChanged("Prohibitmovewarn");
                    if (prohibitmovewarn == true)
                    {
                        this.ProhibitmovewarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.ProhibitmovewarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility prohibitmovewarnV = Visibility.Collapsed;//禁行报警
        public Visibility ProhibitmovewarnV
        {
            get { return prohibitmovewarnV; }
            set
            {
                if (prohibitmovewarnV != value)
                {
                    prohibitmovewarnV = value;
                    this.RaisePropertyChanged("ProhibitmovewarnV");
                }
            }
        }

        private bool illstopwarn = false;//非法停车报警
        public bool Illstopwarn
        {
            get { return illstopwarn; }
            set
            {
                if (illstopwarn != value)
                {
                    illstopwarn = value;
                    this.RaisePropertyChanged("Illstopwarn");
                    if (illstopwarn == true)
                    {
                        this.IllstopwarnV = Visibility.Visible;
                    }
                    else
                    {
                        this.IllstopwarnV = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility illstopwarnV = Visibility.Collapsed;//非法停车报警
        public Visibility IllstopwarnV
        {
            get { return illstopwarnV; }
            set
            {
                if (illstopwarnV != value)
                {
                    illstopwarnV = value;
                    this.RaisePropertyChanged("IllstopwarnV");
                }
            }
        }
        #endregion
    }
}