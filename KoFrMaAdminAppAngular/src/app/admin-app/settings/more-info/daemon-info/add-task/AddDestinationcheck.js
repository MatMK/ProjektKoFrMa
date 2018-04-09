function AddDestination(){
    document.getElementById("AddDest").onclick = function() {
        var inputDestination = document.getElementById("inputDestination");
        var input = document.createElement("input");
        input.type = "text";
        input.name = "inputDestinationCreated";
        var br = document.createElement("br");
        inputDestination.appendChild(input);
        inputDestination.appendChild(br);
    }
}