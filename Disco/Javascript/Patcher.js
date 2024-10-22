{
    // remove elements
    let elements = document.getElementsByClassName("DISCO_STYLING");
    for (let i = 0; i < elements.length; i++) {
        elements[i].parentNode.removeChild(elements[i]);
    }

    // remove old patch
    elements = document.getElementsByClassName("DISCO_PATCH");
    for (let i = 0; i < elements.length; i++) {
        elements[i].parentNode.removeChild(elements[i]);
    }

    // create new patch element
    let patch = document.createElement("meta");
    patch.setAttribute("http-equiv", "content-security-policy");
    patch.setAttribute("content",
        "default-src * 'unsafe-inline' 'unsafe-eval'; script-src * 'unsafe-inline' 'unsafe-eval'; connect-src * 'unsafe-inline'; img-src * data: blob: 'unsafe-inline'; frame-src *; style-src * 'unsafe-inline';");
    patch.setAttribute("class", "DISCO_PATCH");
    document.getElementsByTagName('head')[0].appendChild(patch);

    // create new element
    let element = document.createElement("style");
    element.setAttribute("class", "DISCO_STYLING");

    // inject style
    element.innerHTML = `{{{0}}}`;

    // inject element into HTML
    document.body.append(element);
}