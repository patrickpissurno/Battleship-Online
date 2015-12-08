<?php
$servername = "localhost";
$username = "root";
$password = "toor";
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
						"loses=".$row["loses"]. ";" .
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
                
                $date = date_create();
                $date = date_timestamp_get($date);
                $p1_online = $date;
                $p2_online = $date;
                
                $sql = "SELECT * FROM `matches` WHERE `id`='".$_POST["id"]."'";
				$result = mysqli_query($conn, $sql);

				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
                        $p1 = $row["player1"];
                        $p2 = $row["player2"];
                    }
                }
                
                $sql = "SELECT * FROM `users` WHERE `user`='" . $p1 . "'";
				$result = mysqli_query($conn, $sql);

				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
                        $p1_online = intval($row["last_online"]);
                    }
                }
                
                $sql = "SELECT * FROM `users` WHERE `user`='" . $p2 . "'";
				$result = mysqli_query($conn, $sql);

				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
                        $p2_online = intval($row["last_online"]);
                    }
                }
                
                
                
                if($date - $p1_online > 10 || $date - $p2_online > 10)
                {
                    $sql = "UPDATE `matches` SET `status`='closed' WHERE `id`=" . $_POST["id"];	
                    if(!mysqli_query($conn, $sql))
                        die ("-1");
                }
                
				$sql = "SELECT * FROM `matches` WHERE `id`='".$_POST["id"]."'";
				$result = mysqli_query($conn, $sql);

				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
						echo "player1=".$row["player1"]. ";" .
						"player2=".$row["player2"]. ";" .
						"map1=".$row["map1"]. ";" .
						"map2=".$row["map2"]. ";" .
						"winner=".$row["winner"]. ";" .
                        "p1_ready=".$row["p1_ready"]. ";" .
                        "p2_ready=".$row["p2_ready"]. ";" .
						"status=".$row["status"]. ";" .
                        "turn=".$row["turn"]. ";" .
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
						break;
					}
					if($id != "-1")
					{
						$sql = "UPDATE `matches` SET `player2`='" . $_POST["user"] . "', `status`='running' WHERE `id`=" . $id;
						if(!mysqli_query($conn, $sql))
							die ("-1");
						$sql = "SELECT * FROM `matches` WHERE `id`=" . $id . "";
						$result = mysqli_query($conn, $sql);
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
						}
					}
					else
						echo "-1";
				}
				else
				{
					$sql = "INSERT INTO `matches` (player1, player2, turn, status)
					VALUES ('". $_POST["user"] ."', '', '" . $_POST["user"] . "', 'open')";

					if (mysqli_query($conn, $sql)) {
						$sql = "SELECT * FROM `matches` WHERE `status`='open' and `player1`='" . $_POST["user"] . "'";
						$result = mysqli_query($conn, $sql);
						
						$id = "-1";
						if (mysqli_num_rows($result) > 0) {
								while($row = mysqli_fetch_assoc($result)) {
								$id = $row["id"];
								echo "player1=".$row["player1"]. ";" .
                                "matchid=".$id. ";" . 
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
                
                $sql = "UPDATE `users` SET `wins`=`wins`+1 WHERE `user`='" . $_POST["user"]."'";	
				if(!mysqli_query($conn, $sql))
					die ("-1");
                
                //GET LOOSER
                $sql = "SELECT * FROM `matches` WHERE `id`=" . $_POST["id"];
				$result = mysqli_query($conn, $sql);
				
				$looser = -1;
				if (mysqli_num_rows($result) > 0) {
					// output data of each row
					while($row = mysqli_fetch_assoc($result)) {
						if($row["player2"] == $_POST["user"])
							$looser = $row["player1"];
                        else
                            $looser = $row["player2"];
						break;
					}
				}
                
                $sql = "UPDATE `users` SET `loses`=`loses`+1 WHERE `user`='" . $looser . "'";	
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
        case "write_user_online":
            if(!empty($_POST["user"]))
            {
                $date = date_create();
                $sql = "UPDATE `users` SET `last_online`='" . date_timestamp_get($date) . "' WHERE `user`='" . $_POST["user"]."'";	
				if(!mysqli_query($conn, $sql))
					die ("-1");
            }
            break;
        case "insert_player":
            if(!empty($_POST["user"]) && !empty($_POST["pass"]))
            {
                $sql = "SELECT * FROM `users` WHERE `user`='".$_POST["user"]."'";
				$result = mysqli_query($conn, $sql);
				if (!(mysqli_num_rows($result) > 0)) {
					$sql = "INSERT INTO `users` (user, pass, wins, loses, last_online)
					VALUES ('". $_POST["user"] ."', '" . $_POST["pass"] . "', 0, 0, 0)";
                    if (mysqli_query($conn, $sql)) {
                        die("'" . $_POST["user"] . "' registered successfully.");
                    }
                    else
                        die("Error: can't register '" . $_POST["user"] . "' to the database");
				}
                else
                    die("Username already exists.");
            }
            else
                die("Username/password cannot be empty.");
            break;
        case "register":
            echo '
                <center>
                <h1>BattleShip Online</h1>
                <h3>Register</h3>
                <form action="battleship.php?act=insert_player" method="post">
                    Username:<br>
                    <input type="text" name="user" value="">
                    <br>
                    Password:<br>
                    <input type="password" name="pass" value="">
                    <br><br>
                    <input type="submit" value="Register">
                </form>
            ';
            break;
	}
}
else
{
    echo '
        <center>
        <h1>BattleShip Online</h1>
        <h3>A remake of the classic</h3>
        <a href="battleship.php?act=register">Register</a>
        <a href="#">Download</a>
        </center>
    ';
}

mysqli_close($conn);
?>