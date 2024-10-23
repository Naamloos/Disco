{
    // remove old style elements
    let elements = document.getElementsByClassName("DISCO_STYLE");
    for (let i = 0; i < elements.length; i++) {
        elements[i].parentNode.removeChild(elements[i]);
    }

    // create new style element
    let element = document.createElement("style");
    element.setAttribute("class", "DISCO_STYLE");

    // inject style
    element.innerHTML = `{{{0}}}`;

    // inject element into HTML
    document.body.append(element);
}