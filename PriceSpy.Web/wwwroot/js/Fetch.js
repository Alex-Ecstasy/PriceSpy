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
        })
        .catch(error => console.error(error));
}

form1.addEventListener('keydown', event => {
    if (event.key === 'Enter') {
        event.preventDefault();
        submitForm();
    }
});

form2.addEventListener('keydown', event => {
    if (event.key === 'Enter') {
        event.preventDefault();
        submitForm();
    }
});

submitButton.addEventListener('click', event => {
    event.preventDefault();
    submitForm();
});