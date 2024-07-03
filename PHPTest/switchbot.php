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

$argv = $GLOBALS['argv'];

$token = $argv[1];
$secret = $argv[2];

$url = "https://api.switch-bot.com/v1.1/devices";

$response = request($url, $token, $secret);

$response = getResponseObject($response);
var_dump($response);
echo "\n\n";

$devices = $response->body->deviceList;

foreach ($devices as $device)
{
	$exists = str_contains($device->deviceName, 'Hub Mini');

	if ($exists === false)
	{
		$url = 'https://api.switch-bot.com/v1.1/devices/' .
			$device->deviceId . '/status';

		$response = request($url, $token, $secret);

		$response = getResponseObject($response);
		var_dump($response);
		echo "\n\n";
	}
}
