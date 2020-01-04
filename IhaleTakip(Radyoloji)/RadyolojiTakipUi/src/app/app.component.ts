import { Component } from '@angular/core';
import { DatePipe } from '@angular/common';
import { AuthenticationService } from './entity/service/authentication.service';
import { VWPERSONEL } from './entity/model/vw-personel';
import { registerLocaleData } from '@angular/common';
import localeTr from '@angular/common/locales/tr';

// the second parameter 'fr' is optional
registerLocaleData(localeTr, 'tr');

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Radyoloji İşlem Takibi';
  isLogin: boolean = false;
  username: string;
  sicil_no: number;
  hour: string;

  constructor(private authenticationService: AuthenticationService) {
    if (this.authenticationService.currentUserValue) {
      this.isLogin = true;
      this.username = this.authenticationService.currentUserSubject.value.username;
      this.sicil_no = this.authenticationService.currentUserSubject.value.model.value[0]['sicil_no'];

      let APersonel = new VWPERSONEL();
      APersonel.ihale_cari_kodu = this.authenticationService.currentUserSubject.value.model.value[0]['ihale_cari_kodu'];
      APersonel.ihale_cari_ad_unvan = this.authenticationService.currentUserSubject.value.model.value[0]['ihale_cari_ad_unvan'];
      APersonel.ihale_hastane_cari_kodu = this.authenticationService.currentUserSubject.value.model.value[0]['ihale_hastane_cari_kodu'];
      APersonel.ihale_hastane_cari_ad_unvan = this.authenticationService.currentUserSubject.value.model.value[0]['ihale_hastane_cari_kodu'];
    }
    else {
      this.isLogin = false;
    }
  }

  public formatDate(date) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();

    let hour = d.getHours();
    let minutes = d.getMinutes();
    this.hour = hour + ':' + minutes; 
    console.log(hour + ':' + minutes);
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
  }
 logout(){
   this.authenticationService.logout();
 }
/*   public FormatHour(date) {
    const now = moment("2017-01-26T14:21:22+0000");
    const expiration = moment("2017-01-29T17:24:22+0000");

    // get the difference between the moments
    const diff = expiration.diff(now);

    //express as a duration
    const diffDuration = moment.duration(diff);

    // display
    console.log("Days:", diffDuration.days());
    console.log("Hours:", diffDuration.hours());
    console.log("Minutes:", diffDuration.minutes());
  }
 */}
