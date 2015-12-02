<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "battleship";

// Create connection
$conn = mysqli_connect($servername, $username, $password, $dbname);
// Check connection
if (!$conn) {
    die("-9");
}

if(!empty($_GET["act"]))
{
	switch($_GET["act"])
	{
		case "read_user":
			if(!empty($_POST["user"]))
			{
				$sql = "SELECT * FROM `users` WHERE `user`='".$_POST["user"]."'";
				$result = mysqli_query($conn, $sql);

				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
						echo "user=".$row["user"]. ";" .
						"wins=".$row["wins"]. ";" .
						"loses=".$row["user"]. ";" .
						"";
					}
				} else {
					echo "-1";
				}
			}
			else
				echo "-1";
			break;
		case "check_pass":
			if(!empty($_POST["user"]) && !empty($_POST["pass"]))
			{
				$sql = "SELECT * FROM `users` WHERE `user`='".$_POST["user"]."'";
				$result = mysqli_query($conn, $sql);

				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
						if($_POST["pass"] == $row["pass"])
							echo "1";
						else
							echo "0";
					}
				} else {
					echo "-1";
				}
			}
			else
				echo "-1";
			break;
		case "read_match":
			if(!empty($_POST["id"]))
			{
				$sql = "SELECT * FROM `matches` WHERE `id`='".$_POST["id"]."'";
				$result = mysqli_query($conn, $sql);

				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
						echo "player1=".$row["player1"]. ";" .
						"player2=".$row["player2"]. ";" .
						"map1={".$row["map1"]. "};" .
						"map2={".$row["map2"]. "};" .
						"winner=".$row["winner"]. ";" .
						"status=".$row["status"]. ";" .
						"";
					}
				} else {
					echo "-1";
				}
			}
			break;
		case "matchmaking":
			if(!empty($_POST["user"]))
			{
				$sql = "SELECT * FROM `matches` WHERE `status`='open'";
				$result = mysqli_query($conn, $sql);
				
				$id = "-1";
				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
						$id = $row["id"];
						echo "player1=".$row["player1"]. ";" .
						"player2=".$row["player2"]. ";" .
						"matchid=".$id. ";" .
						"";
						break;
					}
					if($id != "-1")
					{
						$sql = "UPDATE `matches` SET `player2`='" . $_POST["user"] . "', `status`='running' WHERE `id`=" . $id;
						if(!mysqli_query($conn, $sql))
							die ("-1");
					}
					else
						echo "-1";
				}
				else
				{
					$sql = "INSERT INTO `matches` (player1, player2, turn, status)
					VALUES ('". $_POST["user"] ."', '', '" . $_POST["user"] . "', 'open')";

					if (mysqli_query($conn, $sql)) {
						$sql = "SELECT * FROM `matches` WHERE `status`='open' and `player1`='" . $_POST["user"];
						$result = mysqli_query($conn, $sql);
						
						$id = "-1";
						if (mysqli_num_rows($result) > 0) {
								while($row = mysqli_fetch_assoc($result)) {
								$id = $row["id"];
								echo "matchid=".$id. ";" . 
								"";
								break;
							}
						}
					} else {
						echo "Error: " . $sql . "<br>" . mysqli_error($conn);
					}
				}
			}
			break;
		case "write_match_map":
			if(!empty($_POST["id"]) && !empty($_POST["user"]) && !empty($_POST["map"]))
			{
				$sql = "SELECT * FROM `matches` WHERE `id`=" . $_POST["id"];
				$result = mysqli_query($conn, $sql);
				
				$player1 = true;
				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
						if($row["player2"] == $_POST["user"])
							$player1 = false;
						break;
					}
				}
				
				if($player1)
					$sql = "UPDATE `matches` SET `map1`='" . $_POST["map"] . "' WHERE `id`=" . $_POST["id"];
				else
					$sql = "UPDATE `matches` SET `map2`='" . $_POST["map"] . "' WHERE `id`=" . $_POST["id"];
					
					if(!mysqli_query($conn, $sql))
						die ("-1");
			}
			break;
		case "write_match_endturn":
			if(!empty($_POST["id"]) && !empty($_POST["user"]))
			{
				$sql = "SELECT * FROM `matches` WHERE `id`=" . $_POST["id"];
				$result = mysqli_query($conn, $sql);
				
				$next = "-1";
				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
						if($row["player1"] == $_POST["user"])
							$next = $row["player2"];
						else
							$next = $row["player1"];
						break;
					}
				}
				
				if($next != "-1")
				{
					$sql = "UPDATE `matches` SET `turn`='" . $next . "' WHERE `id`=" . $_POST["id"];	
					if(!mysqli_query($conn, $sql))
						die ("-1");
				}
				else
					echo "-1";
			}
			break;
		case "write_match_winner":
			if(!empty($_POST["id"]) && !empty($_POST["user"]))
			{
				$sql = "UPDATE `matches` SET `winner`='" . $_POST["user"] . "', `status`='closed' WHERE `id`=" . $_POST["id"];	
				if(!mysqli_query($conn, $sql))
					die ("-1");
			}
			break;
		case "write_match_ready":
			if(!empty($_POST["id"]) && !empty($_POST["user"]))
			{
				$sql = "SELECT * FROM `matches` WHERE `id`=" . $_POST["id"];
				$result = mysqli_query($conn, $sql);
				
				$player1 = true;
				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
						if($row["player2"] == $_POST["user"])
							$player1 = false;
						break;
					}
				}
				
				if($player1)
					$sql = "UPDATE `matches` SET `p1_ready`=1 WHERE `id`=" . $_POST["id"];
				else
					$sql = "UPDATE `matches` SET `p2_ready`=1 WHERE `id`=" . $_POST["id"];
					
					if(!mysqli_query($conn, $sql))
						die ("-1");
			}
			break;
	}
}

mysqli_close($conn);
?>