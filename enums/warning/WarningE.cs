namespace enums.warning
{
    /// <summary>
    /// 警告状态
    /// </summary>
    public enum WaringStatusE
    {
        Create,
        Resolve
    }

    /// <summary>
    /// 警告类型
    /// </summary>
    public enum WarningTypeE
    {
        DeviceOffline,//设备离线
        TrackFullButNoneStock,//轨道满砖但没库存
        CarrierLoadSortTask,//小车倒库中但是小车有货
        CarrierLoadNotSortTask,//小车倒库中任务清除
        TileNoneStrategy,//砖机没有设置策略
        CarrierFullSignalFullNotOnStoreTrack,//小车满砖信号不在储砖轨道
        CarrierGiveMissTrack,//小车前进放货没有扫到地标
        DownTileHaveNotTrackToStore,//砖机找不到空闲轨道存放
        UpTileHaveNotStockToOut,//砖机找不到库存出库
        TrackEarlyFull,//轨道提前满砖报警
        UpTileHaveNoTrackToOut,//砖机找不到有砖轨道上砖
        FerryNoLocation, //摆渡车没有位置信息
        Warning34,//【反抛未执行】，等待上砖机工位空砖
        Warning35,//【反抛未执行】，上砖侧库存里有反抛任务的品种可上
        Warning36,//【流程超时】
        Warning37,//当前品种设定的上砖数量为零
    }

    public enum CarrierWarnE 
    { 
        ReadConBreakenCheckWire = 100,//RFID阅读器故障
        StoreSlowOverTimeCheckLight,
        FrontAvoidAlert,//前防撞
        BackAvoidAlert,//后防撞
        BackTakeOverTime,//后退取货超时
        FrontGiveOverTime,//前进放货超时
        FrontPointOverTime,
        BackPointOverTime,
        Back2FerryOverTime,
        Front2FerryOverTime,
        GoUpOverTime,
        GoDownOverTime,
        BackTakeCannotDo,
        FrontGiveCannotDo,
        Back2FerryCannotDo,
        Front2FerryCannotDo,
        Back2SortCannotDo,
        Front2PointCannotDo,
        Back2PointCannotDo,
        NotGoodToGoUp,
        SortTaskOverTime,
        FunctinSwitchOverTime
    }

}
