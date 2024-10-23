// dispatcher for patcher response, 0 is substituted for the request ID, 1 is substituted for the response payload
if (typeof DISCO !== 'undefined')
{
    console.log("{{{0}}}");
    DISCO.receiveResponse("{{{0}}}", JSON.parse("{{{1}}}"));
}
else
{
    console.log("DISCO error: DiscoPatcher is not loaded!");
}