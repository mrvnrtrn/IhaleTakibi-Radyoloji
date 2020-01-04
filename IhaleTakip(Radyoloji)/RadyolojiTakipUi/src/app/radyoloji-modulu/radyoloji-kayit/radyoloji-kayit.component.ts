import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Form, Validators } from '@angular/forms';
import { TblIhaleIslem } from 'src/app/entity/model/tbl-ihale-islem';
import { RadyolojiIslemService } from 'src/app/entity/service/radyoloji-islem.service';
import { Router, NavigationEnd } from '@angular/router';
import { getLocaleDateFormat } from '@angular/common';
import { AppComponent } from 'src/app/app.component';
import { Subject } from 'rxjs';
import { AuthenticationService } from 'src/app/entity/service/authentication.service';
import Swal from 'sweetalert2';
declare var $: any;
const now = Date.now();
@Component({
  selector: 'app-radyoloji-kayit',
  templateUrl: './radyoloji-kayit.component.html',
  styleUrls: ['./radyoloji-kayit.component.css']
})
export class RadyolojiKayitComponent implements OnInit {
  RadyolojiListe: any;
  dtOptions: DataTables.Settings = {};
  dtTrigger: Subject<RadyolojiKayitComponent> = new Subject();
  edit_ref_no: number;
  Subscription: any;
  ihale_adi: string;
  isValid: boolean = true;
  filtreleme: boolean = false;
  raportorListesi: any;
  doktorListesi: any;
  tarih: string;
  saat: string;

  constructor(private RadyolojiService: RadyolojiIslemService, private router: Router, private app: AppComponent, private authenticationService: AuthenticationService) {
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    }
    this.Subscription = this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.router.navigated = false;
      }
    });
    this.tarih = this.app.formatDate(now);
    this.saat = this.app.hour;
  }

  fg_radyolojikayit = new FormGroup({
    ihale_cari_kodu: new FormControl(),
    ihale_cari_ad_unvan: new FormControl(),
    ihale_hastane_cari_kodu: new FormControl(),
    ihale_hastane_cari_ad_unvan: new FormControl(),
    hasta_kabul_no: new FormControl(),
    hasta_tc_no: new FormControl('', Validators.required),
    hasta_adi: new FormControl(),
    hasta_soyadi: new FormControl(),
    cekim_tarihi: new FormControl(),
    cekim_saati: new FormControl(),
    islem_kodu: new FormControl(),
    islem_adi: new FormControl(),
    islem_puani: new FormControl(),
    cihaz_adi: new FormControl(),
    rapor_yazim_tarihi: new FormControl(),
    //rapor_yazan_cari_kodu : new FormControl(),
    rapor_yazan_cari_ad_unvan: new FormControl(),
    rapor_onay_tarihi: new FormControl(),
    //rapor_onay_cari_kodu : new FormControl(),
    rapor_onay_cari_ad_unvan: new FormControl()
  })
  fg_datefilter = new FormGroup({
    baslangic_tarihi: new FormControl(),
    bitis_tarihi: new FormControl()
  })

  ngOnInit() {
    this.dtOptions = {
      pagingType: 'first_last_numbers',
      //pageLength: 2
    };

    if (this.filtreleme == false) { this.GetRadyolojiIslemGunluk(); }
    this.GetIhaleAdi();
    this.GetDoktorListesi();
    this.GetRaportorListesi();

    this.fg_radyolojikayit.patchValue({
      cekim_tarihi: this.tarih,
      cekim_saati: this.saat,
      ihale_cari_kodu: this.authenticationService.currentUserSubject.value.model.value[0]['ihale_cari_kodu'],
      ihale_cari_ad_unvan: this.authenticationService.currentUserSubject.value.model.value[0]['ihale_cari_ad_unvan'],
      ihale_hastane_cari_kodu: this.authenticationService.currentUserSubject.value.model.value[0]['ihale_hastane_cari_kodu'],
      ihale_hastane_cari_ad_unvan: this.authenticationService.currentUserSubject.value.model.value[0]['ihale_hastane_cari_ad_unvan'],
      rapor_yazim_tarihi: this.tarih,
      rapor_onay_tarihi: this.tarih,
    })
    this.fg_datefilter.patchValue({
      baslangic_tarihi: this.tarih,
      bitis_tarihi: this.tarih
    })
    //#region TC KİMLİK NUMARASI GEÇERLİLİK KONTROLÜ
    $(document).ready(function () {
      var checkTcNum = function (value) {
        value = value.toString();
        var isEleven = /^[0-9]{11}$/.test(value);
        var totalX = 0;
        for (var i = 0; i < 10; i++) {
          totalX += Number(value.substr(i, 1));
        }
        var isRuleX = totalX % 10 == value.substr(10, 1);
        var totalY1 = 0;
        var totalY2 = 0;
        for (var i = 0; i < 10; i += 2) {
          totalY1 += Number(value.substr(i, 1));
        }
        for (var i = 1; i < 10; i += 2) {
          totalY2 += Number(value.substr(i, 1));
        }
        var isRuleY = ((totalY1 * 7) - totalY2) % 10 == value.substr(9, 0);
        return isEleven && isRuleX && isRuleY;
      };

      $('#hasta_tc_no').on('keyup focus blur load', function (event) {
        event.preventDefault();
        this.isValid = checkTcNum($(this).val());
        console.log('tc ', this.isValid);
        if (this.isValid) {
          $('#hasta_tc_no').attr('class', 'input-sm form-control thresold-i');
        }
        else {
          $('#hasta_tc_no').attr('class', 'input-sm form-control thresold-i invalid');
        }
      });
    });
    //#endregion TC KİMLİK NUMARASI GEÇERLİLİK KONTROLÜ
  }

  SaveRadyolojiIslem(
    ihale_cari_kodu,
    ihale_cari_ad_unvan,
    ihale_hastane_cari_kodu,
    ihale_hastane_cari_ad_unvan,
    hasta_kabul_no,
    hasta_tc_no,
    hasta_adi,
    hasta_soyadi,
    cekim_tarihi,
    cekim_saati,
    islem_kodu,
    islem_adi,
    islem_puani,
    cihaz_adi,
    rapor_yazim_tarihi,
    rapor_yazan_cari_ad_unvan,
    rapor_onay_tarihi,
    rapor_onay_cari_ad_unvan
  ) {
    const ARadyolojiIslem = new TblIhaleIslem();
    if (this.edit_ref_no) { ARadyolojiIslem.ref_no = this.edit_ref_no; } //bir kayıt yapıldığı anlamına geliyor.

    ARadyolojiIslem.ihale_cari_kodu = ihale_cari_kodu;
    ARadyolojiIslem.ihale_cari_ad_unvan = ihale_cari_ad_unvan;
    ARadyolojiIslem.ihale_hastane_cari_kodu = ihale_hastane_cari_kodu;
    ARadyolojiIslem.ihale_hastane_cari_ad_unvan = ihale_hastane_cari_ad_unvan;
    ARadyolojiIslem.hasta_kabul_no = hasta_kabul_no;
    ARadyolojiIslem.hasta_tc_no = hasta_tc_no;
    ARadyolojiIslem.hasta_adi = hasta_adi;
    ARadyolojiIslem.hasta_soyadi = hasta_soyadi;
    ARadyolojiIslem.cekim_tarihi = cekim_tarihi;
    ARadyolojiIslem.cekim_saati = cekim_saati;
    ARadyolojiIslem.islem_kodu = islem_kodu;
    ARadyolojiIslem.islem_adi = islem_adi;
    ARadyolojiIslem.islem_puani = islem_puani;
    ARadyolojiIslem.cihaz_adi = cihaz_adi;
    ARadyolojiIslem.rapor_yazim_tarihi = rapor_yazim_tarihi;
    ARadyolojiIslem.rapor_yazim_tarihi = rapor_yazim_tarihi;
    ARadyolojiIslem.rapor_yazan_cari_ad_unvan = rapor_yazan_cari_ad_unvan;
    ARadyolojiIslem.rapor_onay_tarihi = rapor_onay_tarihi;
    ARadyolojiIslem.rapor_onay_cari_ad_unvan = rapor_onay_cari_ad_unvan;

    this.RadyolojiService.SaveRadyolojiIslem(ARadyolojiIslem).subscribe(data => {
      if (data == 'ok') {
        console.log('Kayıt Başarılı');
        const Toast = Swal.mixin({
          toast: true,
          showConfirmButton: false,
        });
        Toast.fire({
          position: 'center',
          title: 'Kayıt Başarılı',
          icon: 'success',
          timer: 1500
        });
        this.formuTemizle();
        this.router.navigate(["/radyoloji-islem-kayit"]);
      }
    }, error => {
      Swal.fire(
        'Kayıt Başarısız',
        error.error,
        'error')
    });
  }

  /*   
  //sayfada belli bir elemente scroll etme fonksiyonu.
  //kullanımı: button: (click)="scrollToElement(datatableSection)"
  //          element: #datatableSection 
  
    scrollToElement(el: HTMLElement): void {
      console.log(el);
      el.scrollIntoView();  }
      
  */

  formuTemizle() {
    this.fg_radyolojikayit.patchValue({
      ihale_cari_kodu: this.authenticationService.currentUserSubject.value.model.value[0]['ihale_cari_kodu'],
      ihale_cari_ad_unvan: this.authenticationService.currentUserSubject.value.model.value[0]['ihale_cari_ad_unvan'],
      ihale_hastane_cari_kodu: this.authenticationService.currentUserSubject.value.model.value[0]['ihale_hastane_cari_kodu'],
      ihale_hastane_cari_ad_unvan: this.authenticationService.currentUserSubject.value.model.value[0]['ihale_hastane_cari_ad_unvan'],
      hasta_kabul_no: "",
      hasta_tc_no: "",
      hasta_adi: "",
      hasta_soyadi: "",
      cekim_tarihi: this.tarih,
      cekim_saati: this.saat,
      islem_kodu: "",
      islem_adi: "",
      islem_puani: "",
      cihaz_adi: "",
      rapor_yazim_tarihi: this.tarih,
      rapor_onay_tarihi: this.tarih,
      rapor_yazan_raportor: "",
      rapor_onaylayan_doktor: ""
    })
  }

  GetRadyolojiIslem() {
    this.RadyolojiService.GetRadyolojiIslem().subscribe(
      data => {
        console.table('GetRadyolojiIslem=', data);
        this.RadyolojiListe = data;
        this.dtTrigger.next();
      });
  }

  GetIhaleAdi() {
    this.RadyolojiService.GetIhaleAdi(this.app.sicil_no).subscribe(data => {
      this.ihale_adi = String(data);
    })
  }

  GetDoktorListesi() {
    this.RadyolojiService.GetPersonelListesi("DOKTOR").subscribe(data => {
      this.doktorListesi = data;
      console.table(this.doktorListesi);
    })
  }
  GetRaportorListesi() {
    this.RadyolojiService.GetPersonelListesi("RAPORTÖR").subscribe(data => {
      this.raportorListesi = data;
      console.table(this.raportorListesi);
    })
  }

  GetRadyolojiIslemGunluk() {
    this.RadyolojiService.GetRadyolojiIslemGunluk(this.tarih).subscribe(
      data => {
        console.table('GetRadyolojiIslemGunluk=', data);
        this.RadyolojiListe = data;
        this.dtTrigger.next();
      });
  }

  TarihleAra(baslangic_tarihi, bitis_tarihi) {
    this.filtreleme = true;
    this.dtTrigger.unsubscribe();
    this.RadyolojiService.GetRadyolojiIslemFilter(baslangic_tarihi, bitis_tarihi).subscribe(
      data => {
        this.RadyolojiListe = data;
        this.dtTrigger.next();
      });
  }

  KayitEkle() {
    this.router.navigate(["/radyoloji-islem-kayit"]);
  }

  KayitSil(ref_no) {
    const ihaleislem = new TblIhaleIslem();
    ihaleislem.ref_no = ref_no;

    this.RadyolojiService.DeleteIhaleIslem(ihaleislem).subscribe(data => {
      if (data == 'Kayıt Başarıyla Silindi') {
        this.GetRadyolojiIslemGunluk();
      }
    });
  }

  KayitDuzenle(ref_no) {
    if (ref_no != "0") {
      this.RadyolojiService.GetRadyolojiIslemSatiri(ref_no).subscribe(
        data => {
          this.edit_ref_no = data[0]['ref_no'];
          var ihale_cari_kodu = data[0]['ihale_cari_kodu'];
          var ihale_cari_ad_unvan = data[0]['ihale_cari_ad_unvan'];
          var ihale_hastane_cari_kodu = data[0]['ihale_hastane_cari_kodu'];
          var ihale_hastane_cari_ad_unvan = data[0]['ihale_hastane_cari_ad_unvan'];
          var hasta_kabul_no = data[0]['hasta_kabul_no'];
          var hasta_tc_no = data[0]['hasta_tc_no'];
          var hasta_adi = data[0]['hasta_adi'];
          var hasta_soyadi = data[0]['hasta_soyadi'];
          var cekim_tarihi = data[0]['cekim_tarihi'];
          var cekim_saati = data[0]['cekim_saati'];
          var islem_kodu = data[0]['islem_kodu'];
          var islem_adi = data[0]['islem_adi'];
          var islem_puani = data[0]['islem_puani'];
          var cihaz_adi = data[0]['cihaz_adi'];
          var rapor_tarihi = data[0]['rapor_tarihi'];
          var rapor_yazan_raportor = data[0]['rapor_yazan_cari_ad_unvan'];
          var rapor_onaylayan_doktor = data[0]['rapor_onay_cari_ad_unvan'];

          this.fg_radyolojikayit.patchValue({
            ihale_cari_kodu: ihale_cari_kodu,
            ihale_cari_ad_unvan: ihale_cari_ad_unvan,
            ihale_hastane_cari_kodu: ihale_hastane_cari_kodu,
            ihale_hastane_cari_ad_unvan: ihale_hastane_cari_ad_unvan,
            hasta_kabul_no: hasta_kabul_no,
            hasta_tc_no: hasta_tc_no,
            hasta_adi: hasta_adi,
            hasta_soyadi: hasta_soyadi,
            cekim_tarihi: this.app.formatDate(cekim_tarihi),
            cekim_saati: cekim_saati,
            islem_kodu: islem_kodu,
            islem_adi: islem_adi,
            islem_puani: islem_puani,
            cihaz_adi: cihaz_adi,
            rapor_tarihi: this.app.formatDate(rapor_tarihi),
            rapor_yazan_cari_ad_unvan: rapor_yazan_raportor,
            rapor_onay_cari_ad_unvan: rapor_onaylayan_doktor
          })
        }
      )
    }
  }

  ngOnDestroy(): void {
    // Do not forget to unsubscribe the event
    if (this.Subscription) {
      this.Subscription.unsubscribe();
    }
    this.dtTrigger.unsubscribe();
  }
}
