await DISCO.makePatcherRequest({ test: "testing" }).then((response) => {
    // TODO
    console.log(response);
}, () => {
    console.log("test rejected...");
})