// check if a global var "DISCO" exists
class DiscoPatcher {
    constructor(version) {
        this.version = version;

        document.addEventListener("DISCO_DEBUG", (e) => {
            console.log("DISCO_DEBUG: ");
            console.log(e.detail);
        });
    }

    receiveResponse(id, payload) {
        document.dispatchEvent(new CustomEvent(id, { detail: payload }));
    }

    async makePatcherRequest(payload) {
        let id = "DISCO" + Math.random().toString(36).substring(7);

        let requestPayload = {
            id: id,
            payload: payload
        };

        console.log("!__DISCO " + JSON.stringify(requestPayload));

        return await new Promise((resolve, reject) => {
            let resolved = false;
            document.addEventListener(id, (e) => {
                resolved = true;
                document.removeEventListener(id);
                resolve(e.detail)
            });

            // wait for 5 seconds
            setTimeout(() => {
                if (!resolved) {
                    document.removeEventListener(id);
                    reject();
                }
            }, 5000);
        });
    }
}

if (typeof DISCO === 'undefined')
{
    // create the global var
    DISCO = new DiscoPatcher("{{{0}}}");
}