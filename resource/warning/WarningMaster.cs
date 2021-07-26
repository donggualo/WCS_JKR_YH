using enums;
using enums.warning;
using GalaSoft.MvvmLight.Messaging;
using module;
using resource;
using System;
using System.Collections.Generic;

namespace task
{
    public class WarningMaster
    {

        #region[参数/构造]
        private readonly List<Warning> List;
        private DateTime inittime;
        private bool stopwarnadding;

        public WarningMaster()
        {
            List = new List<Warning>();
        }

        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refr_1 = true)
        {
            if (refr_1)
            {
                List.Clear();
                List.AddRange(PubMaster.Mod.WarnSql.QueryWarningList());
            }
            inittime = DateTime.Now;
        }

        public void GetWarns()
        {
            foreach (Warning warning in List)
            {
                SendMsg(warning);
            }
        }

        public void Stop()
        {
            stopwarnadding = true;
        }

        public List<Warning> GetWarnings()
        {
            return List;
        }
        #endregion

        #region[发送信息]

        private void SendMsg(Warning md)
        {
            Messenger.Default.Send(md, MsgToken.WarningUpdate);
        }

        #endregion

        #region[列表操作]
        private void AddWaring(Warning md)
        {
            if (stopwarnadding) return;
            md.id = PubMaster.Dic.GenerateID(DicTag.NewWarnId);
            md.createtime = DateTime.Now;
            List.Add(md);
            SendMsg(md);
            PubMaster.Mod.WarnSql.AddWarning(md);
        }

        private void RemoveWarning(Warning md)
        {
            md.resolve = true;
            md.resolvetime = DateTime.Now;
            SendMsg(md);
            List.Remove(md);
            PubMaster.Mod.WarnSql.EditWarning(md);
        }

        /// <summary>
        /// 获取致命警告
        /// </summary>
        /// <returns></returns>
        public List<Warning> GetFatalError()
        {
            return List;
        }

        public void RemoveWarning(uint warnid)
        {
            Warning warn = List.Find(c => c.id == warnid);
            if(warn != null)
            {
                RemoveWarning(warn);
            }
        }
        #endregion

        #region[设备警告]

        public void AddDevWarn(WarningTypeE warntype, ushort devid, uint transid = 0, uint trackid = 0)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn == null)
            {
                if (stopwarnadding) return;
                if ((DateTime.Now - inittime).TotalSeconds < 20) return;
                warn = new Warning()
                {
                    dev_id = devid,
                    type = (byte)warntype,
                    trans_id = transid,
                    track_id = (ushort)trackid
                };
                string devname = PubMaster.Device.GetDeviceName(devid);
                warn.area_id = (ushort)PubMaster.Device.GetDeviceArea(devid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                if (trackid > 0)
                {
                    string trackname = PubMaster.Track.GetTrackName(trackid);
                    warn.content = devname + ": (" + trackname+") " + warnmsg;
                }
                else
                {
                    warn.content = devname + ": " + warnmsg;
                }
                warn.level = level;
                AddWaring(warn);
            }
        }
        public void RemoveDevWarn(WarningTypeE warntype, ushort devid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        public void RemoveDevWarn(ushort devid)
        {
            Warning warn = List.Find(c => c.dev_id == devid);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        public void AddCarrierWarn(CarrierWarnE warntype, ushort devid, ushort alertidx)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn == null)
            {
                warn = new Warning()
                {
                    dev_id = devid,
                    type = (byte)warntype,
                    track_id = alertidx
                };
                string devname = PubMaster.Device.GetDeviceName(devid);
                warn.area_id = (ushort)PubMaster.Device.GetDeviceArea(devid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                warn.content = devname + ": " + warnmsg;
                warn.level = level;
                AddWaring(warn);
            }
        }
        public void RemoveCarrierWarn(CarrierWarnE warntype, ushort devid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        public void RemoveCarrierWarn(ushort devid, ushort alertidx)
        {
            Warning warn = List.Find(c => c.dev_id == devid && !c.resolve && c.track_id == alertidx);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        #endregion

        #region[任务警告]

        public void AddTaskWarn(uint areaid, WarningTypeE warntype, ushort devid, uint transid = 0, string result = "")
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && c.trans_id == transid && !c.resolve);
            if (warn == null)
            {
                if (stopwarnadding) return;
                if ((DateTime.Now - inittime).TotalSeconds < 20) return;
                warn = new Warning()
                {
                    area_id = (ushort)areaid,
                    dev_id = devid,
                    type = (byte)warntype,
                    trans_id = transid
                };
                string devname = PubMaster.Device.GetDeviceName(devid);
                //warn.area_id = (ushort)PubMaster.Device.GetDeviceArea(devid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                if (devid != 0)
                {
                    warn.content = devname + ": " + warnmsg + " > " + result;
                }
                else
                {
                    warn.content = "任务[" + transid + "] : " + warnmsg + " > " + result;
                }
                warn.level = level;
                AddWaring(warn);
            }
        }


        /// <summary>
        /// 清除任务报警
        /// </summary>
        /// <param name="transid"></param>
        public void RemoveTaskWarn(WarningTypeE warntype, uint transid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.trans_id == transid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        #endregion

        #region[轨道警告]

        public void AddTraWarn(WarningTypeE warntype, ushort trackid, string trackname = null)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.track_id == trackid && !c.resolve);
            if (warn == null)
            {
                warn = new Warning()
                {
                    track_id = trackid,
                    type = (byte)warntype,
                };
                string traname = trackname ?? PubMaster.Track.GetTrackName(trackid);
                warn.area_id = (ushort)PubMaster.Track.GetTrackArea(trackid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                warn.content = traname + ": " + warnmsg;
                warn.level = level;
                AddWaring(warn);
            }
        }

        public void RemoveTraWarn(WarningTypeE warntype, ushort trackid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.track_id == trackid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }

        internal void RemoveTraWarn(ushort trackid)
        {
            Warning warn = List.Find(c => c.track_id == trackid);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }

        #endregion


        #region[判断信息]


        public bool HaveDevWarn(uint devid, ushort level)
        {
            return List.Exists(c => c.dev_id == devid && c.level >= level);
        }

        public bool HaveAreaWarn(uint areaid, ushort level)
        {
            return List.Exists(c => c.area_id == areaid && c.level >= level);
        }

        public bool HaveAreaLineWarn(uint areaid, ushort lineid, ushort level)
        {
            return List.Exists(c => c.area_id == areaid && c.level >= level);
        }
        #endregion
    }
}
