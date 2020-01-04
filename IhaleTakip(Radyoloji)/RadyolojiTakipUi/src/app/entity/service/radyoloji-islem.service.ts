import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { TblIhaleIslem } from '../model/tbl-ihale-islem';

@Injectable({
  providedIn: 'root'
})
export class RadyolojiIslemService {

  constructor(private http: HttpClient, private router: Router) { }
  errorHandler(error: Response) {
    console.log('hatakodu = ' + error);
    return throwError(error);
  }

  GetRadyolojiIslem() {
    return this.http.get(environment.api_url + '/api/ihale-islem/getIhaleIslem', { responseType: 'json' })
  }
  
  GetRadyolojiIslemGunluk(cekim_tarihi) {
    return this.http.get(environment.api_url + '/api/ihale-islem/getIhaleIslemGunluk?cekim_tarihi=' + cekim_tarihi, { responseType: 'json' })
  }

  GetRadyolojiIslemFilter(start,end) {
    return this.http.get(environment.api_url + '/api/ihale-islem/getIhaleIslemFilter?start=' + start +'&end=' + end, { responseType: 'json' })
  }

  GetRadyolojiIslemSatiri(ref_no) {
    return this.http.get(environment.api_url + '/api/ihale-islem/getIhaleIslemSatiri?ref_no=' + ref_no, { responseType: 'json' })
  }

  GetIhaleAdi(sicil_no) {
    return this.http.get(environment.api_url + '/api/ihale-islem/getIhaleAdi?sicil_no=' + sicil_no, { responseType: 'json' })
  }

  GetPersonelListesi(kadro) {
    return this.http.get(environment.api_url + '/api/ihale-islem/getPersonelListesi?kadro=' + kadro, { responseType: 'json' })
  }

  SaveRadyolojiIslem(ARadyolojiIslem: TblIhaleIslem) {
    var header = new HttpHeaders();
    header.append('Content-Type', 'application/json');

    return this.http.post(environment.api_url + '/api/ihale-islem/saveIhaleIslem', ARadyolojiIslem, { headers: header })
      .pipe(
        map(res => res),
        catchError(this.errorHandler)
      );
  }

  DeleteIhaleIslem(ihaleislem: TblIhaleIslem) {
    var header = new HttpHeaders();
    header.append('Content-Type', 'application/json');

    return this.http.post(environment.api_url + '/api/ihale-islem/deleteIhaleIslem', ihaleislem, { headers: header })
      .pipe(
        map(res => res),
        catchError(this.errorHandler)
      );
  }
}
