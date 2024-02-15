/* || ===

= IMKAN.com.sa
= APP FIRE

=== || */


$(document).ready(function () {

    // Featured Slider ..
    $('#top-slider').slick({
        dots: true,
        rtl: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        autoplay: true,
        autoplaySpeed: 5000,
        asNavFor: '.slides .slider',
        appendDots: $(".arrows-move")
    });
    $('.slides .slider').slick({
        dots: false,
        rtl: true,
        asNavFor: '#top-slider',
        arrows: false,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000
    });

    // Partners
    $('.img-list').slick({
        dots: false,
        rtl: true,
        slidesToShow: 7,
        slidesToScroll: 1,
        arrows: true,
        autoplay: true,
        autoplaySpeed: 5000,
        nextArrow: document.getElementById('prev-comp'),
        prevArrow: document.getElementById('next-comp'),
        appendDots: $(".arrows-move"),
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 4,
                    infinite: true,
                    dots: true
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 4
                }
            }

        ]
    });

    // Top
    $('.menuToggle').click(function () {
        $('.mobile-nav ').toggleClass("active");
    });
    $('.site-nav .menu .navTrigger').click(function () {
        $('.site-nav .dropdown').toggleClass("active");
        $(this).toggleClass("active");
    });


    // Tabs ..
    $('.payments-cards article header').click(function () {
        $('.payments-cards article').removeClass("active");
        $(this).parent().addClass("active");
    });

    // Login .. 
    if ($("#login-tabs").length) {
        $('#login-tabs .current-active').click(function () {
            $('#login-tabs').toggleClass('active-register');
        });
    }


    // tabs
    $('#carsLink').click(function () {
        $('.tabs-section .tabs a').parent().addClass("not-now");
        $('.tab-content').removeClass("active");
        $('.tab-content#cars').addClass("active");
        $(this).parent().removeClass("not-now");
    });
    $('#travelLink').click(function () {
        $('.tabs-section .tabs a').parent().addClass("not-now");
        $('.tab-content').removeClass("active");
        $('.tab-content#travel').addClass("active");
        $(this).parent().removeClass("not-now");
    });
    $('#homeLink').click(function () {
        $('.tabs-section .tabs a').parent().addClass("not-now");
        $('.tab-content').removeClass("active");
        $('.tab-content#home').addClass("active");
        $(this).parent().removeClass("not-now");
    });
    $('#medicalLink').click(function () {
        $('.tabs-section .tabs a').parent().addClass("not-now");
        $('.tab-content').removeClass("active");
        $('.tab-content#medical').addClass("active");
        $(this).parent().removeClass("not-now");
    });


    // tabs profile
    $('aside ul li a').click(function () {
        $('.content-view').removeClass("active");
        $('aside ul li').removeClass('active');
        $($(this).data('toshow')).addClass("active");
        $(this).parent().addClass("active");
    });
    $('#profileEditURL').click(function () {
        $('.content.content-view').removeClass("active");
        $('.content#profileEdit').addClass("active");
    });
    $('#addAddressURL').click(function () {
        $('.content.content-view').removeClass("active");
        $('.content#addAddress').addClass("active");
    });
    $('#editsearch').click(function () {
        $('.search-info').removeClass("active");
        $('#edit-search-info').addClass("active");
    });
    $('#showsearch').click(function () {
        $('.search-info').removeClass("active");
        $('#show-search-info').addClass("active");
    });
    $('#editDataLink').click(function () {
        $('.search-result-info').removeClass("active");
        $('#edit-data').addClass("active");
    });
    $('#saveDataLink').click(function () {
        $('.search-result-info').removeClass("active");
        $('#show_data').addClass("active");
    });

    // Sorting Based ..
    $('#show-range-prices').click(function () {
        $('.price-select').toggleClass("active");
    });

    // Show Extra Features ..
    $('.show-extra-features').click(function () {
        $($(this).data('toshow')).toggleClass("active");
        return false;
    });


    // Add new Driver ..
    $('#add-driver').click(function () {
        $('#driverspace').append('<div class="row"><div class="col-xs-12 col-lg-5 col-md-5"><span class="input input--ichiro"><input class="input__field input__field--ichiro" type="text" id="driverid" /><label class="input__label input__label--ichiro" for="driverid"><span class="input__label-content input__label-content--ichiro">رقم الهوية \ الإقامة</span></label></span></div><div class="col-xs-12 col-lg-6 col-md-6"><span class="input input--ichiro"><input class="input__field input__field--ichiro" type="text" id="hijrisample" /><label class="input__label input__label--ichiro" for="hijrisample"><span class="input__label-content input__label-content--ichiro">تاريخ الإنتهاء</span></label></span></div></div> ');
        return false;
    });



    // Filters Show & Hide ..
    $('.toggle-filters').click(function () {
        if ($('#general-view').hasClass('active')) {
            $('#general-view').removeClass("active");
            $('#price-view').removeClass("active");
        } else {
            $('#general-view').addClass("active");
            $('#price-view').addClass("active");
        }
    });

    // Filters Select ..
    $('.select-list li').click(function () {
        $($(this).data('tohide')).removeClass('active');
        $(this).toggleClass("active");
    });

    // Single Features ..
    $('.single-feature:not(.disabled)').click(function () {
        $(this).toggleClass("active");
    });

    // View More ..
    $('article .openMore').click(function () {
        $(this).parent().toggleClass("active");
        $(this).html($(this).text() == 'اخفاء المزيد من التفاصيل' ? ' عرض المزيد من التفاصيل' : 'اخفاء المزيد من التفاصيل');
    });

    // Add To Compare ..
    $('.cd-add-to-cart').click(function () {
        $(this).parent().toggleClass("active");
        $(this).html($(this).text() == '<i class="ic-plus"></i> حذف من المقارنة' ? ' <i class="ic-plus"></i>اضف الي المقارنة' : '<i class="ic-plus"></i> حذف من المقارنة');
    });

    // Show & Hide Password ..
    $('.show-password').hover(function () {
        $('.pass-to-show').attr('type', 'text');
    }, function () {
        $('.pass-to-show').attr('type', 'password');
    });

    // Registred Car ..
    $('.registerCheckClass').change(function () {
        $('.registerdCarInput').toggleClass("active");
        $('.divOwner').toggleClass("switchin");
        if ($('.divOwner').hasClass("switchin")) {
            if ($('#IsOwner').val() == "false") {
                $("input[name='OwnerId']").val("");
                $(".owenerCheck").click();
            }
        }
        $('.registerdCarLabel').text($(this).hasClass('customCard') ? 'Registration Number' : 'Custom Card Number');
    });

    // Owner Check ..
    $('.owenerCheck').change(function () {
        $(".ownerIdInput").toggleClass("active");
    });

    // Add Driver .. 
    $(".addnewdriverCheck").change(function () {
        $(".driver-fields").toggleClass("hidden");
    });

    // Select Update ..
    $('.select2').select2();
    $('.selectinside').click(function () {
        $(this).addClass('clicked');
    });

    // Edit Page ..
    $(function () {
        $('#selectres').change(function () {
            $('.options-list').addClass('hidden');
            $('#' + $(this).val()).removeClass('hidden');
        });
    });

    // Close Modal ..
    $('.closeThis').click(function () {
        $('#CompareModal').removeClass('active');
    });

    // Car Registration ..
    var from = new Pikaday({
        field: document.getElementById('endcarregister'),
        firstDay: 1,
        minDate: new Date(),
        maxDate: new Date(2030, 12, 31),
        format: 'D/M/YYYY',
        toString(date, format) {
            // you should do formatting based on the passed format,
            // but we will just return 'D/M/YYYY' for simplicity
            const day = date.getDate();
            const month = date.getMonth() + 1;
            const year = date.getFullYear();
            return `${day}/${month}/${year}`;
        },
        parse(dateString, format) {
            // dateString is the result of `toString` method
            const parts = dateString.split('/');
            const day = parseInt(parts[0], 10);
            const month = parseInt(parts[1] - 1, 10);
            const year = parseInt(parts[1], 10);
            return new Date(year, month, day);
        }
    });



    // Car Registration ..
    var GregorianYear = (new Date()).getFullYear();
    var HijriYear = Math.round((GregorianYear - 622) * (33 / 32));
    var from = new Pikaday({
        field: document.getElementById('hijrisample'),
        firstDay: 1,
        minDate: new Date(1420, 1, 1),
        maxDate: new Date(1441, 12, 31),
        yearRange: [1420, 1441],
        format: 'D/M/YYYY',
        toString(date, format) {
            // you should do formatting based on the passed format,
            // but we will just return 'D/M/YYYY' for simplicity
            const day = date.getDate();
            const month = date.getMonth() + 1;
            const year = date.getFullYear();
            return `${day}/${month}/${year}`;
        },
        parse(dateString, format) {
            // dateString is the result of `toString` method
            const parts = dateString.split('/');
            const day = parseInt(parts[0], 10);
            const month = parseInt(parts[1] - 1, 10);
            const year = parseInt(parts[1], 10);
            return new Date(year, month, day);
        },
        // Hijri Converting ..
        i18n: {
            months: ['محرم', 'صفر', 'ربيع الأول', 'ربيع الثاني', 'جمادي الأول', 'جمادي الثاني', 'رجب', 'شعبان', 'رمضان', 'شوال', 'ذو القعدة', 'ذو الحجة'],
            weekdays: ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'],
            weekdaysShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
        }
    });








    // Images Slider ..
    $(function () {
        $('#slider-steps').slick({
            dots: false,
            rtl: true,
            slidesToShow: 1,
            slidesToScroll: 1,
            arrows: true,
            autoplay: false,
            nextArrow: document.getElementById('prev-step'),
            prevArrow: document.getElementById('next-step')
        });
    });

});




