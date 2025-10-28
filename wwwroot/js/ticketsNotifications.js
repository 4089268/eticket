document.addEventListener("DOMContentLoaded", async () => {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/ticketsHub")
        .build();

    await connection.start();

    connection.on("ReceiveNotification", (message, folio) => {
        showNotification(message, folio);
    });

    function showNotification(message, folio)
    {

        if (typeof Swal !== "undefined") {
            Swal.fire({
                toast: true,
                position: "top-end",
                topLayer: true,
                icon: "info",
                title: message,
                showConfirmButton: true,
                confirmButtonText: 'Mostrar Reporte',
                backdrop: true,
                timer: 5000,
                timerProgressBar: true,
                target:"main",
            })
            .then((result)=>{
                if(result.isConfirmed)
                {
                    window.location.href = `/Reportes/${folio}`
                }
            });
        } else {
            alert(message);
        }
    }
});