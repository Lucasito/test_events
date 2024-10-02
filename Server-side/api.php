<?
$data = json_decode(file_get_contents('php://input'), true);
// checking input data, example exist data['events']
foreach ( $data['events'] as $key => $value ) {
    save_to_log("type: ".$value['type'].", data: ".$value['data']);
}
function save_to_log($msg)
{
    file_put_contents('log.txt', date("y-m-d H:i:s: ").$msg . "\n", FILE_APPEND);
} 
?>