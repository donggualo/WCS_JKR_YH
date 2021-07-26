# WcsFixPlatform[ 储砖调度系统 ]




## 更新日志：

# **[2020-10-27 : 上砖机按轨道上砖 2.0]**
## 更新内容：
1. 砖机按轨道上砖，可以同时添加多条同一个轨道【例：1_01_出，1_02_出，1_01_出】
2. 平板-摆渡车对位，选择摆渡车改为弹框选择，为了避免误操作
3. 分配摆渡车时，如果同时有多个车的站点为同一个摆渡车则不分配


## 数据库更新:

1. 砖机轨道ID
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`
, `bool_value`) VALUES (53, 1, 'NewTileTrackId', '生成砖机轨道ID', NULL, b'0')
```

2. 修改砖机轨道表结构
```mysql
ALTER TABLE `tile_track` ADD COLUMN `id` int(11) UNSIGNED NOT NULL COMMENT '砖机轨道' FIRST;
ALTER TABLE `tile_track`DROP PRIMARY KEY;
ALTER TABLE `tile_track` ADD PRIMARY KEY (`id`) ;
ALTER TABLE `tile_track` DROP COLUMN `enable`;
```


# **[2020-10-23 : 上砖机按轨道上砖]**
## 更新内容：
1. 按轨道上砖
2. 出库任务分配小车，查找在取货轨道，后退取砖任务，对应站点为入轨道的车
3. 修复库存规格时间修改后，没有保存到数据库

## 背景：
> 在下砖和上砖规格频繁切换的情况下，减少上砖机的规格频繁设置。
### 
## 解决方案:
1. 在设备表添加作业模式(work_type)，砖机设备使用. 值：0代表按规格上砖， 1代码按轨道上砖；
2. 添加砖机上砖轨道表(tile_track: tile_id, track_id,order, enable);
3. 上砖设置使用按轨道上砖后，根据上砖轨道分配中，一条一条轨道的分配上砖。

## 操作说明
1. 右键修改砖机策略，修改上砖为【按轨作业】
2. 在打开按轨出库模块，选择砖机，还没有轨道信息的，点击弹框确定添加
3. 初次添加的轨道信息，默认添加到非作业轨道类别中
4. 在非作业轨道列表中，选中需要上砖的轨道，点击【←】按钮把轨道添加到作业轨道列表
5. 取消则在作业轨道列表选中轨道后点击【→】
6. 调整轨道上砖顺序，选中需要调整的轨道，然后点击上移/下移
7. 在没有点击【保存】按钮前的修改都不会起作用，修改后需要点击保存。才起作用。

## 数据库更新:
1. 设备表添加作业模式(work_type)，砖机设备使用. 值：0代表按规格上砖， 1代码按轨道上砖
```mysql
ALTER TABLE `device` ADD COLUMN `work_type` tinyint(3) 
UNSIGNED NULL COMMENT '作业类型\r\n砖机：0按规格 1按轨道' AFTER `do_work`;
```
2. 添加砖机上砖轨道表

```mysql
CREATE TABLE `tile_track` (
  `tile_id` int(11) unsigned NOT NULL COMMENT '砖机ID',
  `track_id` int(11) unsigned NOT NULL,
  `order` smallint(5) unsigned DEFAULT NULL COMMENT '优先级',
  `enable` bit(1) DEFAULT NULL COMMENT '启用停用',
  PRIMARY KEY (`tile_id`,`track_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
```
3. 添加字典数据
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, 
`string_value`) VALUES (52, 3, 'UpTileHaveNoTrackToOut', '砖机找不到轨道上砖', NULL, b'', 
'砖机找不到轨道上砖')
```


# **[2020-10-18 : 提前满砖警告]**
## 背景：
> 小车在没有读到2002的情况下提前给了满砖的信息导致轨道提前满砖
### 
## 警告预防:
1. 在【区域表】添加该区域的轨道最少满砖数量(full_qty)，这个数量不会做强制限制，设置为0则不警告该区域的轨道提前满砖情况
2. 在【轨道表】添加提前满砖信号，满砖时间 
3. 小车给了满砖信号，计算该轨道库存数量和区域设置的满砖数量对比，小于区域设置的数量则报警，同时设置轨道提前满砖状态和满砖时间。
4. 倒库时，检查轨道是否是提前满砖并且满砖时间距离当前时间是否超过了5分钟，是则开始倒该轨道
5. 轨道提前满砖警告后，设置轨道为有砖，或者停用则清空警告信息。

## 数据库更新:
1. 轨道添加提前满砖信息，满砖时间
```mysql
ALTER TABLE `track` ADD COLUMN `early_full` bit NULL COMMENT '提前满砖' AFTER `alert_trans`;
ALTER TABLE `track` ADD COLUMN `full_time` datetime NULL COMMENT '满砖时间' AFTER `early_full`;
```
2. 区域表添加 轨道未达到满砖数

```mysql
ALTER TABLE `area` ADD COLUMN `full_qty` tinyint UNSIGNED NULL COMMENT '轨道未达到满砖警告数' AFTER `carriertype`;
```
3. 添加检查轨道是否提前满砖报警信息
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`,
`double_value`, `uint_value`, `order`, `updatetime`) VALUES (51, 3, 'TrackEarlyFull', '请检查轨道是否提前满砖了',
NULL, b'0', '请检查轨道是否提前满砖了', NULL, NULL, NULL, NULL);
```



# **[2021-07-20: 报警灯逻辑]**

## 报警添加线路字段，等级字段

```mysql
ALTER TABLE `warning` ADD COLUMN `level` TINYINT(3) UNSIGNED NULL COMMENT '等级';
```

## 报警字典添加等级

```mysql
ALTER TABLE `diction_dtl` ADD COLUMN `level` tinyint(3) UNSIGNED NULL COMMENT '等级';
```

# **[2021-07-26:添加流程报警信息]**

## 添加字典
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (100, 3, 'Warning36', '【流程超时】', NULL, NULL, '【流程超时】', NULL, NULL, NULL, NULL, 3);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (101, 4, 'StepOverTime', '除【倒库中】，其他流程的超时时间（秒）', 600, NULL, NULL, NULL, NULL, NULL, '2021-06-30 08:44:37', 3);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (102, 4, 'SortingStockStepOverTime', '倒库中流程的超时时间（秒）', 7200, NULL, NULL, NULL, NULL, NULL, NULL, 3);
```
