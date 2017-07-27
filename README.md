# fusilicious
A Battlefield-style first person shooter with 5 classes and team oriented AI with an emphasis on communication and lack of randomness for 10 vs 10 online or offline play.

<!-- language: lang-none -->

    ////////////////////////////////////////////////////////++++++++++++++++++++++++++++++++++++++++++++
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ::::::::::::::::::::::://///////////////////////////////////////////////////////////////////////////
    :::::::::::::os+:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://////
    ::::::::::::::+sdy+::/ys::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    :::::::::::::::::ohdsyh/::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    ------------------::hdd+::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    ---------------------::oys/:+s/---------------------------------------------------------------------
    -------------------------/hdmhms/-------------------------------------------------------------------
    ---------------------------:+syhhdds/--------------------------:::::::::----------------------------
    ..............................-/mmmmmho:.................:+sso//:::---------:---....................
    ................................odNNmmmdho-............./ssssyNNmdhyo+:-------::--..................
    ..................................:sdNNmmmms/........../sssyyddmNNNNmmddhs+:---:::-.................
    ````````````````````````````````````./ymNNmmmdo-``````+ssyyo/:::::/osydmmmmmhs/::+/-````````````````
    ```````````````````````````````````````.+hmmNNNmy/.```//++//:::---....-:+ydNNNNdyss/.```````````````
    ``````````````````````````````````````````:ymmNNmmds+/:++///::::----.....-:/sdNNyss/-```````````````
                                             `-hmmmNNmNNNN++++////:::::--.-----:::sysyNm:```````````````
                                           .//ooyddmydNmmmsoo++++/////::::::::::://ssyNNy               
                                        ./sdmysss/.o-+dmmmdooooo+++++/////////////++++yN/               
                                        -mmmmds+/`  `-hhhhyyssssooooo++++++++++++++oooos                
                                         :dy/.  `    :hyhy`.-oyyssssoooooooooooosyyss+:`                
                                          ``         :ddy-   `+hhhhhyyyyyyyyyyyhmNNh:`                  
       ```````````````````````````````````````````````-/-`````:NNNNNNNNNNNmNNNNNNmNN.```````````````````
    ----------------------------------------------------------+NNNNNNNNNNNNNNNNNmNNNosy+-:://///////////
    ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::+NNNNNNNNNNNNNNNmmmNNmmmds:+ssssssssssssss
    ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::+NNNNNNNNNNNNNNNNmmmNmy+/::+ssssssssssssss
    ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::+NNNNNNNNNNNNNNNNmmNNN/::::+ssssssssssssss
    ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::oNNNNNNNNNNNNNNNNmNNmm/:://ossssssssssssss
    ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::oNNNNNNNNNNNNNNNNdmNmmo+++oossssssssssssss
    :::::::::::::::::::::::::::::::::::::::::::::::::::::::/:/oNNNNNNNNNNNNNNNmmNNNmmmdho+///+++++++++++
    //////////////////////////////////////////////////////////oNNNNNNNNNNNNNNNmNNNNmdmmmmho/////////////
    //////////////////////////////////////////////////////////oNNNNNNNNNNNNNNNNNNNNN+oydmh//////////////
    //////////////////////////////////////////////////////////oNNNNNNNNNNNNNNNNNmmNm///+o+//////////////
    //////////////////////////////////////////////////////////oNNNNNNNNNNNNNNNNNmmNm////////////////////
    //////////////////////////////////////////////////////////oNNMNNNNNNNNNNNNmNmmNm////////////////////
    //////////////////////////////////////////////////////////oNNMNNNNNNNNNNNNmmmmNm////////////////////
    //////////////////////////////////////////////////////////oNNMMNNNNNNNNNNNmmmmNm////////////////////
    //////////////////////////////////////////////////////////oNNMMNNNNNNNNNNNmmmmNm////////////////////
    ////////////////////++/+++++++++++++++++++++++++++++++++++sNNMMNNNNNNNNNNNNmNNNd++++++++++++++++++++
    ++++++++++++++++++++++++++++++++++++++++++++++++++++osssssyNMMMMNNNNNNNNNNNNNNNd++++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++++++oooooooooosssssssyNMMMMNNNNNNNNNNNNNNNd++++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++++++++oosooossoossssssshdmNNNNNNNNNNNmmNdyo++++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++++++++o++++++++++ssssso++//++++++++++/+d///++++++++++++++++++++
    ++++++++++++++++++++++++++++++++++++++++++++++++++++++oyyyyo++///:::::-::::+d///++++++++++++++++++++
    ++++++++++++++++++++++++++++++++++++++++++++++++++++++osyyyo++///::::::::osdN+//++++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++syyyo++///::::::::++om//+++++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++oyyyo++///::::::::::om//+++++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++oyyyo++///::::::::::od//+++++++++++++++++++++
    ++++++++++++++++++++++++++++++++++++++++++++++++++++++++oyyoo++//::::::::::/+//+++++++++++++++++++++
    ++++++++++++++++++++++++++++++++++++++++++++++++++++++++oyyso++///:::::::::////+++++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++syyoo++///:::::::////++++++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++oooo++++++++++++++++oyyso++/////:::://///++o+++++++++++++++++++
    ++++++++++ooo+++++++++++oooooooooooooooooooooooooooooooooooyyso++///////////++oooooooooooooooooooo++
    ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooymdys++//////+oydsoooooooooooooooooooooo
    oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooosyddNNNmddddNNmdsooooooooooooooooooooooo
    oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooossssssssssso+ooooooooooooooooooooooooo
    oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
