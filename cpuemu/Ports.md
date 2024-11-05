make ports for transferring data.

pin 0 is live pin. if pin 0 is 1, device is connected

connection protocol
wait for live pin to change
if live pin changes, set pin 1 to request metadata about device
you can set special registers to tell the system where certain devices are, and it will use them in related operations ie. set the GFXPORT register to the port address of your graphics processing unit, or set the VCPORT register to the address of your video interface for cpu rendered graphics instead of a gpu