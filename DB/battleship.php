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
	switch($act)
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
						"";
						break;
					}
				}
				
				if($id != "-1")
				{
					$sql = "UPDATE `matches` SET `player2`='" . $_POST["user"] . "', `status`='running' WHERE `id`=" . $id;
					if(!mysqli_query($conn, $sql))
						die ("-1");
				}
				else
				{
					echo "-1";
				}
			}
			break;
	}
}

mysqli_close($conn);
?>