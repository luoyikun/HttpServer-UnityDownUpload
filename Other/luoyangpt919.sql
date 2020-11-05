/*
Navicat MySQL Data Transfer

Source Server         : luoyangpt919
Source Server Version : 50717
Source Host           : localhost:3306
Source Database       : luoyangpt919

Target Server Type    : MYSQL
Target Server Version : 50717
File Encoding         : 65001

Date: 2020-11-05 18:34:03
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for luoyangpt919
-- ----------------------------
DROP TABLE IF EXISTS `luoyangpt919`;
CREATE TABLE `luoyangpt919` (
  `id` text NOT NULL,
  `name` text,
  `path` text,
  `type` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`(256))
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of luoyangpt919
-- ----------------------------
INSERT INTO `luoyangpt919` VALUES ('1', '', '12312', '0');
