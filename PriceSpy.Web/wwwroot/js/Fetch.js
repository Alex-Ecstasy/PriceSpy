//function sendData() {
//    var input = document.querySelector("#search-box2").value;
//    var rate = document.querySelector("#rate-box2").value;
//    var data = { param1: input, param2: rate };
//};
//fetch('/Home/Prices', {
//    method: 'Post',
//    headers: {
//        'Content-Type': 'applicarion/json'
//    },
//    body: JSON.stringify(data)
//})
//    .then(function (response) {
//        return response.text();
//    })
//    .then(function (result) {
//        document.querySelector('#accordionPanelsStayOpenExample').innerHTML = result;
//    });
    
//document.querySelector('#search-button2').addEventListener('click', function (e) {
//    e.preventDefault();
//    sendData();
//});

//document.querySelector('#search-box2').addEventListener('keydown', function (e) {
//    if (e.key === 'Enter') {
//        e.preventDefault();
//        sendData();
//    }

//});

//document.querySelector('#rate-box2').addEventListener('keydown', function (e) {
//    if (e.key === 'Enter') {
//        e.preventDefault();
//        sendData();
//    }

//});

const form1 = document.querySelector('#search-box2');
const form2 = document.querySelector('#rate-box2');
const submitButton = document.querySelector('#search-button2');

const submitForm = () => {
    const param1 = form1.value;
    const param2 = form2.value;

    fetch('/Home/Fetch/?searchQuery=' + param1 + '&Rate=' + param2, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ param1, param2 })
    })
        .then(function (response) {
            return response.text();
        })
        .then(function (result) {
            document.querySelector('#accordionPanelsStayOpenExample').innerHTML = result;
        });
        /*.catch(error => console.error(error))*/;
}

//form1.addEventListener('keydown', event => {
//    if (event.key === 'Enter') {
//        event.preventDefault();
//        submitForm();
//    }
//});

//form2.addEventListener('keydown', event => {
//    if (event.key === 'Enter') {
//        event.preventDefault();
//        submitForm();
//    }
//});

submitButton.addEventListener('click', event => {
    event.preventDefault();
    submitForm();
});