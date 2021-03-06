﻿//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Utilities;
using Tunynet.Common;

namespace Spacebuilder.Bar
{
    /// <summary>
    /// 帖吧动态接收人获取器
    /// </summary>
    public class BarSectionActivityReceiverGetter : IActivityReceiverGetter
    {
        

        /// <summary>
        /// 获取接收人UserId集合
        /// </summary>
        /// <param name="activityService">动态业务逻辑类</param>
        /// <param name="ownerId">动态拥有者Id</param>
        /// <param name="userId">动态发布者Id</param>
        /// <param name="activityItemKey">动态项目标识</param>
        /// <returns></returns>
        IEnumerable<long> IActivityReceiverGetter.GetReceiverUserIds(ActivityService activityService, Activity activity)
        {
            SubscribeService subscribeService = new SubscribeService(TenantTypeIds.Instance().BarSection());
            IEnumerable<long> followerUserIds = subscribeService.GetUserIdsOfObject(activity.OwnerId);
            if (followerUserIds == null)
                return new List<long>();
            return followerUserIds.Where(n => IsReceiveActivity(activityService, n, activity));
        }

        /// <summary>
        /// 检查用户是否接收动态
        /// </summary>
        /// <param name="activityService"></param>
        /// <param name="userId">UserId</param>
        /// <param name="activity">动态</param>
        /// <returns>接收动态返回true，否则返回false</returns>
        private bool IsReceiveActivity(ActivityService activityService, long userId, Activity activity)
        {
            //检查用户是否已在信息发布者的粉丝圈里面
            FollowService followService = new FollowService();
            if (followService.IsFollowed(userId, activity.UserId))
                return false;
            //检查用户是否接收该动态项目
            Dictionary<string, bool> userSettings = activityService.GetActivityItemUserSettings(userId);
            if (userSettings.ContainsKey(activity.ActivityItemKey))
                return userSettings[activity.ActivityItemKey];
            else
            {
                //如果用户没有设置从默认设置获取
                ActivityItem activityItem = activityService.GetActivityItem(activity.ActivityItemKey);
                if (activityItem != null)
                    return activityItem.IsUserReceived;
                else
                    return true;
            }
        }

    }
}
