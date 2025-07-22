document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("searchInput");
    const brandFilter = document.getElementById("brandFilter");
    function loadCars(page = 1) {
        const search = searchInput.value;
        const brandId = brandFilter.value;
        fetch(`${baseUrl}?page=${page}&search=${encodeURIComponent(search)}&brandId=${encodeURIComponent(brandId)}`, {
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            }
        })
            .then(res => res.text())
            .then(html => {
                document.getElementById("carResults").innerHTML = html;

                // Re-bind pagination links
                document.querySelectorAll("#carResults .page-link").forEach(link => {
                    link.addEventListener("click", function (e) {
                        e.preventDefault();
                        const page = new URL(link.href).searchParams.get("page");
                        loadCars(page);
                    });
                });
            })
            .catch(err => console.log(err));
    }

    searchInput.addEventListener("input", () => loadCars(1));
    brandFilter.addEventListener("change", () => loadCars(1));
});