<?php

function getByte($text)
{
	$byte = ord($text);

	return $byte;
}

function getResponseObject($response)
{
	$object = json_decode($response);

	return $object;
}

function guidv4($data = null)
{
    // Generate 16 bytes (128 bits) of random data or
	// use the data passed into the function.
    $data = $data ?? random_bytes(16);
    assert(strlen($data) == 16);

	$data[6] = chr(ord($data[6]) & 0x0f | 0x40);
    $data[8] = chr(ord($data[8]) & 0x3f | 0x80);

	$data[6] = updateCharacter($data[6], 0x0f, 0x40);
	$data[8] = updateCharacter($data[8], 0x3f, 0x80);

    // Output the 36 character UUID.
    $text = vsprintf('%s%s-%s-%s-%s-%s%s%s', str_split(bin2hex($data), 4));

	$hex = bin2hex($data);
	$parts = str_split($hex, 4);
    $text = vsprintf('%s%s-%s-%s-%s-%s%s%s', $parts);

	return $text;
}

function postSensorData($data)
{
	$url = 'https://vanilla.kitchen/api/sensors';

	$jsonData = json_encode($data);

	$curl = curl_init($url);

	$options =
	[
		'Content-Type: application/json'
	];

	curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($curl, CURLOPT_POST, true);
	curl_setopt($curl, CURLOPT_HTTPHEADER, $options);
	curl_setopt($curl, CURLOPT_POSTFIELDS, $jsonData);

	$response = curl_exec($curl);

	$httpCode = curl_getinfo($curl, CURLINFO_HTTP_CODE);
    
    if ($response === false)
	{
		$error = curl_error($curl);
		$result = ['error' => 'cURL error: ' . $error];
	}
	else
	{
		$result =
		[
			'status' => $httpCode,
			'response' => json_decode($response, true)
		];
	}

	curl_close($curl);


	return $result;
}

function request($url, $token, $secret)
{
	$nonce = guidv4();
	$time = time() * 1000;

	$rawData = $token . $time . $nonce;
	$data = updateEncoding($rawData);

	$sign = hash_hmac('sha256', $data, $secret, true);
	$sign = base64_encode($sign);
	$sign = strtoupper($sign);

	$curl = curl_init($url);
	curl_setopt($curl, CURLOPT_URL, $url);
	curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);

	$headers =
	[
		"Content-Type:application/json",
		"Authorization:" . $token,
		"sign:" . $sign,
		"nonce:" . $nonce,
		"t:" . $time
	];

	curl_setopt($curl, CURLOPT_HTTPHEADER, $headers);
	$response = curl_exec($curl);
	curl_close($curl);

	return $response;
}

function updateCharacter($character, $mask1, $mask2)
{
	$item = $character;
	$interim = getByte($item);
	$interim = $interim & $mask1 | $mask2;
	$text = chr($interim);
	
	return $text;
}

function updateEncoding($text)
{
	$encoding = mb_detect_encoding($text);
	$encoding = mb_convert_encoding($text, 'UTF-8', $encoding);
 
	 return $encoding;
}

/****************************************************************************/
// Begin
$argv = $GLOBALS['argv'];

$token = '9f0067f6446c17fb2759a732e1a063e524eb2586b444dd60d80941af17f7598cdbc93274374f9277d5f32a8ad877b5cb'; $secret = '2f28e8988dac3d763acdd5dca5dcfecc';

$url = "https://api.switch-bot.com/v1.1/devices";

$now = date('Y-m-d H:i:s');
echo PHP_EOL . 'Sending at: ' . $now . PHP_EOL;

$response = request($url, $token, $secret);

$response = getResponseObject($response);
// var_dump($response);
// echo "\n\n";

$isObject = is_object($response);

if ($isObject === true)
{
	$devices = $response->body->deviceList;

	$finish = false;
	$index = 0;

	foreach ($devices as $device)
	{
		$exists = str_contains($device->deviceName, 'Hub Mini');

		if ($exists === false)
		{
			$url = 'https://api.switch-bot.com/v1.1/devices/' .
				$device->deviceId . '/status';

			$response = request($url, $token, $secret);

			$response = getResponseObject($response);
			// var_dump($response);
			// echo "\n\n";

			$isObject = is_object($response);

			if ($isObject === true)
			{
				$body = $response->body;
				$jsonData = json_encode($body);
				echo 'Sending Data: ' . $jsonData . PHP_EOL;
			}

			$result = postSensorData($body);
			echo 'Data Sent to Vanilla - Result: ' . $result['status'] . PHP_EOL;
			echo "\n\n";

			sleep(15);
		}
	}
}
