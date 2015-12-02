-- phpMyAdmin SQL Dump
-- version 4.3.11
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: 02-Dez-2015 às 15:21
-- Versão do servidor: 5.6.24
-- PHP Version: 5.6.8

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `battleship`
--

-- --------------------------------------------------------

--
-- Estrutura da tabela `matches`
--

CREATE TABLE IF NOT EXISTS `matches` (
  `id` int(9) NOT NULL,
  `player1` varchar(30) NOT NULL,
  `player2` varchar(30) NOT NULL,
  `p1_ready` int(1) NOT NULL,
  `p2_ready` int(1) NOT NULL,
  `map1` varchar(150) NOT NULL,
  `map2` varchar(150) NOT NULL,
  `turn` varchar(30) NOT NULL,
  `winner` varchar(15) NOT NULL,
  `status` varchar(30) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;

--
-- Extraindo dados da tabela `matches`
--

INSERT INTO `matches` (`id`, `player1`, `player2`, `p1_ready`, `p2_ready`, `map1`, `map2`, `turn`, `winner`, `status`) VALUES
(1, 'teste', 'teste', 0, 0, '', '', 'teste', '', 'running'),
(2, 'teste', 'teste', 0, 0, '', '', 'teste', '', 'running'),
(3, 'teste', 'teste2', 0, 0, '', '', 'teste', '', 'running'),
(4, 'teste', 'teste2', 0, 0, '', '', 'teste', '', 'running');

-- --------------------------------------------------------

--
-- Estrutura da tabela `users`
--

CREATE TABLE IF NOT EXISTS `users` (
  `id` int(9) NOT NULL,
  `user` varchar(30) NOT NULL,
  `pass` varchar(30) NOT NULL,
  `wins` int(9) NOT NULL,
  `loses` int(9) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

--
-- Extraindo dados da tabela `users`
--

INSERT INTO `users` (`id`, `user`, `pass`, `wins`, `loses`) VALUES
(1, 'teste', '123', 0, 0),
(2, 'teste2', '123', 0, 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `matches`
--
ALTER TABLE `matches`
  ADD UNIQUE KEY `id` (`id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD UNIQUE KEY `id` (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `matches`
--
ALTER TABLE `matches`
  MODIFY `id` int(9) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=5;
--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(9) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=3;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
