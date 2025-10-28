document.addEventListener("DOMContentLoaded", async () => {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/ticketsHub")
        .build();

    await connection.start();

    connection.on("ReceiveNotification", (message, actionLink) => {
        showNotification(message, actionLink);
    });

    function showNotification(message, actionLink)
    {

        if (typeof Swal !== "undefined")
        {
            Swal.fire({
                toast: true,
                position: "bottom-end",
                topLayer: true,
                icon: "info",
                title: message,
                showConfirmButton: true,
                confirmButtonText: 'Mostrar Reporte',
                backdrop: true,
                timer: 7000,
                timerProgressBar: true,
                target:"main",
            })
            .then((result)=>{
                if(result.isConfirmed)
                {
                    window.location.href = actionLink
                }
            });
        } else {
            alert(message);
        }
    }
});