export interface IInsuranceCompany {
  id: number;
  nameAR: string;
  nameEN: string;
  descAR: string;
  descEN: string;
  namespaceTypeName: string;
  classTypeName: string;
  reportTemplateName: string;
  createdDate: Date;
  createdBy: string;
  lastModifiedDate: Date;
  modifiedBy: string;
  address: Address;
  contact: Contact;
  isActive: boolean;
  key:string;
}

interface Address {
  id: number;
  title: string;
  address1: string;
  address2: string;
  objLatLng: string;
  buildingNumber: string;
  street: string;
  district: string;
  city: string;
  postCode: string;
  additionalNumber: string;
  regionName: string;
  polygonString: string;
  isPrimaryAddress: string;
  unitNumber: string;
  latitude: string;
  longitude: string;
  cityId: string;
  regionId: string;
  restriction: string;
  pKAddressID: string;
  driverId: string;
  addressLoction: string;
}

interface Contact {
  id: number;
  mobileNumber: string;
  homePhone: string;
  fax: string;
  email: string;
}