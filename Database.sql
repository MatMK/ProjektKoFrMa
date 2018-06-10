-- phpMyAdmin SQL Dump
-- version 4.2.9.1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jun 10, 2018 at 05:31 PM
-- Server version: 5.5.55-0+deb7u1
-- PHP Version: 5.4.45-0+deb7u8

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `3b1_kocourekmatej_db2`
--
CREATE DATABASE IF NOT EXISTS `3b1_kocourekmatej_db2` DEFAULT CHARACTER SET utf8 COLLATE utf8_czech_ci;
USE `3b1_kocourekmatej_db2`;

-- --------------------------------------------------------

--
-- Table structure for table `tbAdminAccounts`
--

CREATE TABLE IF NOT EXISTS `tbAdminAccounts` (
`Id` int(11) NOT NULL,
  `Username` varchar(100) COLLATE utf8_czech_ci NOT NULL,
  `Email` varchar(200) COLLATE utf8_czech_ci NOT NULL,
  `Enabled` bit(1) NOT NULL,
  `Password` varchar(100) COLLATE utf8_czech_ci NOT NULL,
  `Token` varchar(100) COLLATE utf8_czech_ci DEFAULT NULL
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbDaemons`
--

CREATE TABLE IF NOT EXISTS `tbDaemons` (
`Id` int(11) NOT NULL,
  `Version` int(3) NOT NULL,
  `OS` varchar(50) COLLATE utf8_czech_ci NOT NULL,
  `PC_Unique` varchar(100) COLLATE utf8_czech_ci NOT NULL,
  `Allowed` tinyint(1) NOT NULL DEFAULT '1',
  `LastSeen` datetime DEFAULT NULL,
  `Password` varchar(200) COLLATE utf8_czech_ci NOT NULL,
  `Token` varchar(200) COLLATE utf8_czech_ci NOT NULL,
  `TimerTick` int(11) NOT NULL DEFAULT '120',
  `TimerOnStart` int(11) NOT NULL DEFAULT '2',
  `TimerAfterFail` int(11) NOT NULL DEFAULT '60'
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbEmailPreferences`
--

CREATE TABLE IF NOT EXISTS `tbEmailPreferences` (
`Id` int(11) NOT NULL,
  `IdAdmin` int(11) NOT NULL,
  `SendOnlyFailed` bit(1) NOT NULL,
  `RepeatInJSON` varchar(2000) COLLATE utf8_czech_ci DEFAULT NULL,
  `SendImmediatelyAfterServerError` bit(1) NOT NULL,
  `RecievingEmail` varchar(1000) COLLATE utf8_czech_ci NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbPermissions`
--

CREATE TABLE IF NOT EXISTS `tbPermissions` (
`Id` int(11) NOT NULL,
  `Permission` int(11) NOT NULL,
  `IdAdmin` int(11) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=352 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbRestApiExceptions`
--

CREATE TABLE IF NOT EXISTS `tbRestApiExceptions` (
`Id` int(100) NOT NULL,
  `ExceptionInJson` varchar(2000) COLLATE utf8_czech_ci NOT NULL,
  `TimeOfException` date NOT NULL,
  `AdminNotified` bit(1) NOT NULL DEFAULT b'0',
  `Severity` int(5) DEFAULT NULL
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbRestApiExceptionsAdminNOTNotified`
--

CREATE TABLE IF NOT EXISTS `tbRestApiExceptionsAdminNOTNotified` (
`Id` int(11) NOT NULL,
  `IdRestApiExceptions` int(11) NOT NULL,
  `IdAdmin` int(11) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbTasks`
--

CREATE TABLE IF NOT EXISTS `tbTasks` (
`Id` int(11) NOT NULL,
  `IdDaemon` int(11) NOT NULL,
  `Task` varchar(10000) COLLATE utf8_czech_ci NOT NULL,
  `TimeOfExecution` datetime NOT NULL,
  `IdPreviousTask` int(11) NOT NULL,
  `BackupTypePlan` varchar(20) COLLATE utf8_czech_ci NOT NULL,
  `RepeatInJSON` varchar(5000) COLLATE utf8_czech_ci DEFAULT NULL,
  `Completed` bit(1) NOT NULL DEFAULT b'0'
) ENGINE=InnoDB AUTO_INCREMENT=132 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbTasksCompleted`
--

CREATE TABLE IF NOT EXISTS `tbTasksCompleted` (
`Id` int(11) NOT NULL,
  `IdDaemon` int(11) NOT NULL,
  `IdTask` int(11) NOT NULL,
  `BackupJournal` longtext COLLATE utf8_czech_ci NOT NULL,
  `TimeOfCompletition` datetime NOT NULL,
  `DebugLog` longtext COLLATE utf8_czech_ci NOT NULL,
  `IsSuccessfull` bit(1) NOT NULL,
  `AdminNotified` bit(1) NOT NULL DEFAULT b'0'
) ENGINE=InnoDB AUTO_INCREMENT=116 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbTasksCompletedAdminNOTNotified`
--

CREATE TABLE IF NOT EXISTS `tbTasksCompletedAdminNOTNotified` (
`Id` int(11) NOT NULL,
  `IdTaskCompleted` int(11) NOT NULL,
  `IdAdmin` int(11) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=77 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tbAdminAccounts`
--
ALTER TABLE `tbAdminAccounts`
 ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `tbDaemons`
--
ALTER TABLE `tbDaemons`
 ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `tbEmailPreferences`
--
ALTER TABLE `tbEmailPreferences`
 ADD PRIMARY KEY (`Id`), ADD UNIQUE KEY `IdAdmin` (`IdAdmin`);

--
-- Indexes for table `tbPermissions`
--
ALTER TABLE `tbPermissions`
 ADD PRIMARY KEY (`Id`), ADD KEY `IdAdmin` (`IdAdmin`);

--
-- Indexes for table `tbRestApiExceptions`
--
ALTER TABLE `tbRestApiExceptions`
 ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `tbRestApiExceptionsAdminNOTNotified`
--
ALTER TABLE `tbRestApiExceptionsAdminNOTNotified`
 ADD PRIMARY KEY (`Id`), ADD UNIQUE KEY `Id` (`Id`), ADD KEY `Id_2` (`Id`), ADD KEY `Id_3` (`Id`);

--
-- Indexes for table `tbTasks`
--
ALTER TABLE `tbTasks`
 ADD PRIMARY KEY (`Id`), ADD KEY `IdPreviousTask` (`IdPreviousTask`), ADD KEY `IdDaemon` (`IdDaemon`);

--
-- Indexes for table `tbTasksCompleted`
--
ALTER TABLE `tbTasksCompleted`
 ADD PRIMARY KEY (`Id`), ADD KEY `IdDaemon` (`IdDaemon`), ADD KEY `IdDaemon_2` (`IdDaemon`), ADD KEY `IdDaemon_3` (`IdDaemon`), ADD KEY `IdDaemon_4` (`IdDaemon`);

--
-- Indexes for table `tbTasksCompletedAdminNOTNotified`
--
ALTER TABLE `tbTasksCompletedAdminNOTNotified`
 ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tbAdminAccounts`
--
ALTER TABLE `tbAdminAccounts`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=46;
--
-- AUTO_INCREMENT for table `tbDaemons`
--
ALTER TABLE `tbDaemons`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=10;
--
-- AUTO_INCREMENT for table `tbEmailPreferences`
--
ALTER TABLE `tbEmailPreferences`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=31;
--
-- AUTO_INCREMENT for table `tbPermissions`
--
ALTER TABLE `tbPermissions`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=352;
--
-- AUTO_INCREMENT for table `tbRestApiExceptions`
--
ALTER TABLE `tbRestApiExceptions`
MODIFY `Id` int(100) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=57;
--
-- AUTO_INCREMENT for table `tbRestApiExceptionsAdminNOTNotified`
--
ALTER TABLE `tbRestApiExceptionsAdminNOTNotified`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=38;
--
-- AUTO_INCREMENT for table `tbTasks`
--
ALTER TABLE `tbTasks`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=132;
--
-- AUTO_INCREMENT for table `tbTasksCompleted`
--
ALTER TABLE `tbTasksCompleted`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=116;
--
-- AUTO_INCREMENT for table `tbTasksCompletedAdminNOTNotified`
--
ALTER TABLE `tbTasksCompletedAdminNOTNotified`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=77;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
